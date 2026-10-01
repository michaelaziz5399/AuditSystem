namespace AuditServer.DataBase.Dtos.AuthDtos
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public DateTime ExpiresOn { get; set; }
        public UserDetailsDto? User { get; set; }
    }
}
