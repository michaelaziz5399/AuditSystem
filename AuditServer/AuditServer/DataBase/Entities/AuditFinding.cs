using AuditServer.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    public class AuditFinding
    {
        [Key]
        public int Id { get; set; }

        // ربط المخالفة بالمهمة مباشرة (إجباري لجميع أنواع المهام)
        public int MissionId { get; set; }
        public AuditMission Mission { get; set; } = null!;

        public int? MissionClientSampleId { get; set; }
        public MissionClientSample? MissionClientSample { get; set; }

        // نوع المحور الرئيسي
        public FindingCategory Category { get; set; } // FieldVisit, Financial, Documentary

        // قيم الـ Enums القابلة للإلغاء (حسب النوع المختار)
        public FieldVisitViolation? FieldViolation { get; set; }
        public FinancialViolation? FinancialViolation { get; set; }
        public DocumentaryViolation? DocumentaryViolation { get; set; }

        // التعليق الخاص بالمخالفة المحددة
        public string? Comment { get; set; }

        public FindingSeverity Severity { get; set; } = FindingSeverity.Medium;
        public FindingStatus Status { get; set; } = FindingStatus.Draft;

        [Required]
        public string CreatedByAuditorId { get; set; } = string.Empty;
        public ApplicationUser CreatedByAuditor { get; set; } = null!;
    
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<AuditAttachment> Attachments { get; set; } = new List<AuditAttachment>();
    }
}
