using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// جدول المحافظات
    /// </summary>
    public class Governorate
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int RegionId { get; set; }
        public Region Region { get; set; } = null!;

        // Navigation Properties
        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    }
}
