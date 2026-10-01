using System.ComponentModel.DataAnnotations;

namespace AuditServer.DataBase.Entities
{
    /// <summary>
    /// المستندات والملفات المرفقة بالملاحظات
    /// </summary>
    public class AuditAttachment
    {
        [Key]
        public int Id { get; set; }

        public int AuditFindingId { get; set; }
        public AuditFinding AuditFinding { get; set; } = null!;

        [Required, MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
