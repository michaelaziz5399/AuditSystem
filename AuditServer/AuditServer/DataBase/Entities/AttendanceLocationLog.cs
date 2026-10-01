using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// نقاط التتبع الجغرافي اللحظية المسجلة خلال الـ 8 ساعات
    /// </summary>
    public class AttendanceLocationLog
    {
        [Key]
        public int Id { get; set; }

        public int DailyAttendanceId { get; set; }
        public DailyAttendance DailyAttendance { get; set; } = null!;

        [Column(TypeName = "decimal(18,8)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal Longitude { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}
