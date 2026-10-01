using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// جدول المستخدمين المربوط بنظام ASP.NET Core Identity
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

        // الربط بالهيكل الإداري (قسم مراجعة / تحقيق / إدارة عامة)
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        // تعديل: الربط بأكثر من منطقة جغرافية (Many-to-Many)
        public ICollection<UserRegion> UserRegions { get; set; } = new List<UserRegion>();

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<MissionAuditor> MissionAuditors { get; set; } = new HashSet<MissionAuditor>();
        public ICollection<MissionOfficerAssignment> MissionOfficerAssignments { get; set; } = new HashSet<MissionOfficerAssignment>();
        public ICollection<MissionClientSample> AssignedClientSamples { get; set; } = new HashSet<MissionClientSample>();
        public ICollection<AuditFinding> CreatedFindings { get; set; } = new HashSet<AuditFinding>();
        public ICollection<DailyAttendance> Attendances { get; set; } = new HashSet<DailyAttendance>();
        public ICollection<AuditorDelegation> DelegationsGiven { get; set; } = new HashSet<AuditorDelegation>();
        public ICollection<AuditorDelegation> DelegationsReceived { get; set; } = new HashSet<AuditorDelegation>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    }
}
