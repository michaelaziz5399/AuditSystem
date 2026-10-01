using AuditServer.DataBase.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// الانتدابات المؤقتة للمراجعين بين الأقاليم
    /// </summary>
    public class AuditorDelegation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DelegatorId { get; set; } = string.Empty;
        public ApplicationUser Delegator { get; set; } = null!;

        [Required]
        public string DelegateeId { get; set; } = string.Empty;
        public ApplicationUser Delegatee { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DelegationStatus Status { get; set; } = DelegationStatus.Pending;
        public string Reason { get; set; } = string.Empty;
    }
}
