using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Dtos.AuthDtos
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 خانات")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare(nameof(NewPassword), ErrorMessage = "كلمة المرور الجديدة غير متطابقة")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
