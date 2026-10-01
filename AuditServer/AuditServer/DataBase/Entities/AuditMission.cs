using AuditServer.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// مهمات المراجعة الميدانية
    /// </summary>
    public class AuditMission
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty; // مثال: "جرد خزينة فرع أسيوط - Q3"
        public MissionType Type { get; set; } // نوع المهمة

        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public int? RegionalDirectiveId { get; set; }
        public RegionalDirective? RegionalDirective { get; set; }

        public AuditMissionStatus Status { get; set; } = AuditMissionStatus.Planned;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation Properties
        public ICollection<MissionAuditor> MissionAuditors { get; set; } = new List<MissionAuditor>();
        public ICollection<MissionOfficerAssignment> OfficerAssignments { get; set; } = new List<MissionOfficerAssignment>();
        public ICollection<MissionClientSample> ClientSamples { get; set; } = new List<MissionClientSample>();
    }
}
