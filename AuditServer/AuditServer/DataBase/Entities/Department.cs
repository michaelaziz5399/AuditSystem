using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// جدول الإدارات والأقسام الفرعية (هيكل شجري Self-Referencing)
    /// يتيح التمييز بين إدارة المراجعة وإدارة التحقيقات والإدارة العامة
    /// </summary>
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // معرف الإدارة الرئيسية التابع لها (في حالة الأقسام الفرعية)
        public int? ParentDepartmentId { get; set; }
        public Department? ParentDepartment { get; set; }

        // Navigation Properties
        public ICollection<Department> SubDepartments { get; set; } = new List<Department>();
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
