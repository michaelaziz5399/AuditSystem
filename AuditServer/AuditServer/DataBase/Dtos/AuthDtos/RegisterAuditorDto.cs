using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Dtos.AuthDtos
{
    public class RegisterAuditorDto
    {
        [Required(ErrorMessage = "الاسم بالكامل مطلوب"), MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "كود الموظف مطلوب"), MaxLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب"), EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "القسم الإداري مطلوب")]
        public int DepartmentId { get; set; }

        // قائمة المناطق الجغرافية المنسوبة للمراجع (يمكن أن تكون أكثر من منطقة)
        public List<int> RegionIds { get; set; } = new();

        // قائمة أدوار المراجع (مثل: AuditManager, SeniorAuditor, FieldAuditor)
        [Required(ErrorMessage = "يجب تحديد دور واحد على الأقل للمراجع")]
        public List<string> Roles { get; set; } = new();
    }
}
