using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// جدول البيانات الأساسية للعملاء
    /// </summary>
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public int OperationsOfficerId { get; set; }
        public BranchOperationsOfficer OperationsOfficer { get; set; } = null!;

        // Navigation Properties
        public ICollection<MissionClientSample> MissionSamples { get; set; } = new List<MissionClientSample>();
    }
}
