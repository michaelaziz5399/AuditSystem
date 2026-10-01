using AuditServer.DataBase.Dtos.AuthDtos;
using AuditServer.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuditServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthRepo _authRepo) : ControllerBase
    {
        // 1. تسجيل مراجع جديد (يتطلب أن يكون تسجيل الدخول باسم مدير مراجعة أو مدير إدارة فرعية)
        [HttpPost("register")]
        [Authorize(Roles = "AuditManager,SubDepartmentManager")]
        public async Task<IActionResult> Register([FromBody] RegisterAuditorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized(new { message = "غير مصرح، تعذر العثور على هوية المستخدم الحالي." });

            var result = await _authRepo.RegisterAsync(dto, currentUserId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 2. تسجيل الدخول
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authRepo.LoginAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 3. تجديد الـ Token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authRepo.RefreshTokenAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 4. تسجيل الخروج (إبطال الـ Refresh Token)
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { message = "رمز الـ Refresh Token مطلوب" });

            var isLoggedOut = await _authRepo.LogoutAsync(refreshToken);

            if (!isLoggedOut)
                return BadRequest(new { message = "الرمز غير صالح أو ملغى بالفعل" });

            return Ok(new { message = "تم تسجيل الخروج بنجاح" });
        }

        // 5. تغيير كلمة المرور
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isChanged = await _authRepo.ChangePasswordAsync(userId, dto);

            if (!isChanged)
                return BadRequest(new { message = "فشل تغيير كلمة المرور، يرجى التأكد من كلمة المرور الحالية" });

            return Ok(new { message = "تم تغيير كلمة المرور بنجاح" });
        }

        // 6. التنظيف اليدوي للتوكنز المنتهية (صلاحية خاصة بالمشرف)
        [HttpDelete("clean-expired-tokens")]
        [Authorize(Roles = "AuditManager")]
        public async Task<IActionResult> CleanExpiredTokens()
        {
            var deletedCount = await _authRepo.RevokeExpiredRefreshTokensAsync();
            return Ok(new { message = $"تم مسح {deletedCount} توكن منتهي بنجاح" });
        }
    }
}