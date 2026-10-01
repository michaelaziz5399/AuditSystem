using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// توزيع محفظة مسؤولي العمليات بالفروع على المراجعين بالمهمة
    /// </summary>
    public class MissionOfficerAssignment
    {
        [Key]
        public int Id { get; set; }

        public int MissionId { get; set; }
        public AuditMission Mission { get; set; } = null!;

        public int OperationsOfficerId { get; set; }
        public BranchOperationsOfficer OperationsOfficer { get; set; } = null!;

        public string AssignedAuditorId { get; set; } = string.Empty;
        public ApplicationUser AssignedAuditor { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
