using AuditServer.DataBase.Dtos.AuthDtos;

namespace AuditServer.Services.AuthService
{
    public interface IAuthRepo
    {
        Task<AuthResponseDto> RegisterAsync(RegisterAuditorDto dto, string currentUserId);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<int> RevokeExpiredRefreshTokensAsync();
    }
}
