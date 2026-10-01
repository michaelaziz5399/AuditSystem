using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// جلسة الحضور اليومي للمراجع بالفرع (Check-in/Check-out)
    /// </summary>
    public class DailyAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AuditorId { get; set; } = string.Empty;
        public ApplicationUser Auditor { get; set; } = null!;

        public int MissionId { get; set; }
        public AuditMission Mission { get; set; } = null!;

        public DateTime AttendanceDate { get; set; } = DateTime.UtcNow.Date;
        public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
        public DateTime? CheckOutTime { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal CheckInLatitude { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal CheckInLongitude { get; set; }

        public bool IsWithinGeofence { get; set; } = true;
        public bool IsCompleted { get; set; } = false;

        // Navigation Properties
        public ICollection<AttendanceLocationLog> LocationLogs { get; set; } = new List<AttendanceLocationLog>();
    }
}
