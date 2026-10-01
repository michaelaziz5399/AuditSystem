using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// جدول الأقاليم الجغرافية
    /// </summary>
    public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<Governorate> Governorates { get; set; } = new List<Governorate>();
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<RegionalDirective> RegionalDirectives { get; set; } = new List<RegionalDirective>();
    }
}
