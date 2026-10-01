using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// جدول الفروع مع إحداثيات الـ Geofencing
    /// </summary>
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string BranchCode { get; set; } = string.Empty;

        public int GovernorateId { get; set; }
        public Governorate Governorate { get; set; } = null!;

        // إحداثيات الفرع الجغرافية وحيز الحضور المسموح به
        [Column(TypeName = "decimal(18,8)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Longitude { get; set; }

        public double AllowedRadiusInMeters { get; set; } = 200.0;

        // Navigation Properties
        public ICollection<BranchOperationsOfficer> OperationsOfficers { get; set; } = new List<BranchOperationsOfficer>();
        public ICollection<AuditMission> AuditMissions { get; set; } = new List<AuditMission>();
    }
}
