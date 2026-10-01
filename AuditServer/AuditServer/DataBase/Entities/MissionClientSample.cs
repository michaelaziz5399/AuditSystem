using AuditServer.DataBase.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// جدول عينات العملاء المستخرجة من شيت الاكسيل والمرتبطة بالمهمة
    /// </summary>
    public class MissionClientSample
    {
        [Key]
        public int Id { get; set; }

        public int MissionId { get; set; }
        public AuditMission Mission { get; set; } = null!;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public string? AssignedAuditorId { get; set; }
        public ApplicationUser? AssignedAuditor { get; set; }

        public SampleAuditStatus Status { get; set; } = SampleAuditStatus.Pending;

        public string? AuditNotes { get; set; }
        public DateTime? AuditedAt { get; set; }

        // --- إحداثيات وتوثيق موقع فحص/زيارة العينة ---

        [Column(TypeName = "decimal(18,8)")]
        public decimal? VisitLatitude { get; set; } // خط العرض وقت توثيق العينة

        [Column(TypeName = "decimal(18,8)")]
        public decimal? VisitLongitude { get; set; } // خط الطول وقت توثيق العينة

        public DateTime? LocationRecordedAt { get; set; } // توقيت تسجيل الموقع الجغرافي

        // Navigation Properties
        public ICollection<AuditFinding> Findings { get; set; } = new List<AuditFinding>();
    }
}
