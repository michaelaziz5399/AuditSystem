namespace AuditServer.DataBase.Enums
{
    // حالة فحص عينة العميل
    public enum SampleAuditStatus
    {
        Pending = 1,
        AuditedClean = 2,
        AuditedWithFindings = 3,
        Excluded = 4,
        Substituted = 5
    }
}
