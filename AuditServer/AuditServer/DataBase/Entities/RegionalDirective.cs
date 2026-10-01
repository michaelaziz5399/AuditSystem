using AuditServer.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// التكليفات الإقليمية الصادرة من الإدارة العليا
    /// </summary>
    public class RegionalDirective
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public int RegionId { get; set; }
        public Region Region { get; set; } = null!;

        public DirectiveStatus Status { get; set; } = DirectiveStatus.Draft;
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<AuditMission> Missions { get; set; } = new List<AuditMission>();
    }
}
