using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Dtos.AuthDtos
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
