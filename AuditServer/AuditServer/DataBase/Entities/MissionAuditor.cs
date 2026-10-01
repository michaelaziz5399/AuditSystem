namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// تسكين المراجعين ورؤساء الفرق على المهمات (Junction Table)
    /// </summary>
    public class MissionAuditor
    {
        public int MissionId { get; set; }
        public AuditMission Mission { get; set; } = null!;

        public string AuditorId { get; set; } = string.Empty;
        public ApplicationUser Auditor { get; set; } = null!;

        public bool IsLeader { get; set; } = false;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
