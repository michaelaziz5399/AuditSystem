using AuditServer.DataBase.DBContext;
using AuditServer.DataBase.Dtos.AuthDtos;
using AuditServer.DataBase.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuditServer.Services.AuthService
{
    public class AuthRepo(UserManager<ApplicationUser> _userManager,
       RoleManager<IdentityRole> _roleManager,
        AppDbContext _context,
        IMapper _mapper,
        IConfiguration _configuration) : IAuthRepo
    {
        // 1. تسجيل مراجع جديد (حسّاس للصلاحيات + دعم أكثر من Role و Region)
        public async Task<AuthResponseDto> RegisterAsync(RegisterAuditorDto dto, string currentUserId)
        {
            // أ) التحقق من أن من ينشئ الحساب هو مدير إدارة فرعية أو مدير مراجعة
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
                return new AuthResponseDto { IsSuccess = false, Message = "المستخدم غير موجود" };

            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            bool isAuthorizedCreator = currentUserRoles.Contains("AuditManager") || currentUserRoles.Contains("SubDepartmentManager");

            if (!isAuthorizedCreator)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "غير مصرح لك بإنشاء حسابات جديدة. هذه الصلاحية تقتصر على مدير المراجعة ومدير الإدارة الفرعية فقط."
                };
            }

            // ب) التحقق من عدم تكرار كود الموظف أو البريد أو اسم المستخدم
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return new AuthResponseDto { IsSuccess = false, Message = "البريد الإلكتروني مسجل بالفعل" };

            if (await _userManager.FindByNameAsync(dto.UserName) != null)
                return new AuthResponseDto { IsSuccess = false, Message = "اسم المستخدم مسجل بالفعل" };

            if (await _context.Users.AnyAsync(u => u.EmployeeCode == dto.EmployeeCode))
                return new AuthResponseDto { IsSuccess = false, Message = "كود الموظف مسجل بالفعل" };

            // ج) تجهيز كائن ApplicationUser
            var user = _mapper.Map<ApplicationUser>(dto);

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return new AuthResponseDto { IsSuccess = false, Message = errors };
            }

            // د) إسناد الأدوار (Roles)
            foreach (var role in dto.Roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            // هـ) إسناد المناطق الجغرافية (Regions)
            if (dto.RegionIds != null && dto.RegionIds.Any())
            {
                var userRegions = dto.RegionIds.Select(regionId => new UserRegion
                {
                    UserId = user.Id,
                    RegionId = regionId
                }).ToList();

                await _context.Set<UserRegion>().AddRangeAsync(userRegions);
                await _context.SaveChangesAsync();
            }

            // و) إرجاع الاستجابة بنجاح
            var userDetailsDto = await GetUserDetailsDtoAsync(user);
            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "تم إنشاء حساب المراجع بنجاح",
                User = userDetailsDto
            };
        }

        // 2. تسجيل الدخول (اسم المستخدم أو كود الموظف)
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.UserRegions)
                .ThenInclude(ur => ur.Region)
                .FirstOrDefaultAsync(u => u.UserName == dto.UsernameOrEmployeeCode || u.EmployeeCode == dto.UsernameOrEmployeeCode);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new AuthResponseDto { IsSuccess = false, Message = "اسم المستخدم/كود الموظف أو كلمة المرور غير صحيحة" };
            }

            if (!user.IsActive)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "هذا الحساب معطل، يرجى مراجعة إدارة النظام" };
            }

            // قراءة مدة الصلاحية بالساعات (افتراضي 3 ساعات)
            var expiryHours = double.TryParse(_configuration["JWT:ExpiryInHours"], out var hours) ? hours : 3;

            // إنشاء JWT Token و Refresh Token
            var jwtToken = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = DateTime.UtcNow.AddHours(expiryHours), // ضبط الصلاحية بـ 3 ساعات
                UserId = user.Id
            };

            await _context.Set<RefreshToken>().AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            var userDetailsDto = await GetUserDetailsDtoAsync(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "تم تسجيل الدخول بنجاح",
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn = jwtToken.ValidTo,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenEntity.ExpiresOn,
                User = userDetailsDto
            };
        }

        // 3. تجديد الـ Token (Refresh Token)
        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            var principal = GetPrincipalFromExpiredToken(dto.Token);
            if (principal == null)
                return new AuthResponseDto { IsSuccess = false, Message = "رمز الأمان غير صالح" };

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.UserRegions)
                .ThenInclude(ur => ur.Region)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || !user.IsActive)
                return new AuthResponseDto { IsSuccess = false, Message = "المستخدم غير موجود أو غير نشط" };

            var existingRefreshToken = await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken && r.UserId == userId);

            if (existingRefreshToken == null || !existingRefreshToken.IsActive)
                return new AuthResponseDto { IsSuccess = false, Message = "جلسة العمل منتهية، يرجى إعادة تسجيل الدخول" };

            // إبطال الـ Refresh Token القديم وإنشاء جديد
            existingRefreshToken.RevokedOn = DateTime.UtcNow;

            var expiryHours = double.TryParse(_configuration["JWT:ExpiryInHours"], out var hours) ? hours : 3;

            var newJwtToken = await GenerateJwtTokenAsync(user);
            var newRefreshToken = GenerateRefreshToken();

            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresOn = DateTime.UtcNow.AddHours(expiryHours), // ضبط الصلاحية بـ 3 ساعات
                UserId = user.Id
            };

            await _context.Set<RefreshToken>().AddAsync(newRefreshTokenEntity);
            await _context.SaveChangesAsync();

            var userDetailsDto = await GetUserDetailsDtoAsync(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "تم تجديد الجلسة بنجاح",
                Token = new JwtSecurityTokenHandler().WriteToken(newJwtToken),
                ExpiresOn = newJwtToken.ValidTo,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiration = newRefreshTokenEntity.ExpiresOn,
                User = userDetailsDto
            };
        }

        // 4. تسجيل الخروج (إبطال الـ Refresh Token)
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var tokenEntity = await _context.Set<RefreshToken>().FirstOrDefaultAsync(r => r.Token == refreshToken);
            if (tokenEntity == null || !tokenEntity.IsActive) return false;

            tokenEntity.RevokedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // 5. تغيير كلمة المرور
        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            return result.Succeeded;
        }
        // 6. دالة تنظيف وحذف الـ Refresh Tokens المنتهية أو الملغاة
        public async Task<int> RevokeExpiredRefreshTokensAsync()
        {
            // المقارنة بـ UtcNow تضمن مراعاة فرق التوقيت عالمياً ومحلياً بدون أي خلل
            var expiredOrRevokedTokens = await _context.Set<RefreshToken>()
                .Where(r => r.ExpiresOn <= DateTime.UtcNow || r.RevokedOn != null)
                .ToListAsync();

            if (!expiredOrRevokedTokens.Any())
                return 0;

            _context.Set<RefreshToken>().RemoveRange(expiredOrRevokedTokens);
            return await _context.SaveChangesAsync();
        }

        // ================= Helpers =================

        private async Task<JwtSecurityToken> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Name, user.UserName!),
        new(ClaimTypes.Email, user.Email!),
        new("EmployeeCode", user.EmployeeCode),
        new("FullName", user.FullName),
        new("DepartmentId", user.DepartmentId.ToString()),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

            // قراءة الساعات من Configuration مع القيمة الافتراضية 3 ساعات
            var expiryHours = double.TryParse(_configuration["JWT:ExpiryInHours"], out var hours) ? hours : 3;

            return new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(expiryHours), // 3 ساعات
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)),
                ValidateLifetime = false // لتسهيل قراءة الـ Claims من الـ Token المنتهي
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }

        private async Task<UserDetailsDto> GetUserDetailsDtoAsync(ApplicationUser user)
        {
            var dto = _mapper.Map<UserDetailsDto>(user);
            dto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            return dto;
        }
    }
}

