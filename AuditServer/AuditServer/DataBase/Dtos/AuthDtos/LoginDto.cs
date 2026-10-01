using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Dtos.AuthDtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "اسم المستخدم أو كود الموظف مطلوب")]
        public string UsernameOrEmployeeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        public string Password { get; set; } = string.Empty;
    }
}
