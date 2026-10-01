namespace AuditServer.DataBase.Enums
{
    // حالة مهمة المراجعة الميدانية
    public enum AuditMissionStatus
    {
        Planned = 1,
        InProgress = 2,
        PendingReview = 3,
        Completed = 4,
        Cancelled = 5
    }
}
