using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// جدول مسؤولي العمليات بالفروع (مسؤولي المحافظ)
    /// </summary>
    public class BranchOperationsOfficer
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<MissionOfficerAssignment> MissionAssignments { get; set; } = new List<MissionOfficerAssignment>();
    }
}
