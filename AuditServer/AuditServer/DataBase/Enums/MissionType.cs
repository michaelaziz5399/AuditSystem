namespace AuditServer.DataBase.Enums
{
    public enum MissionType
    {
        ClientPortfolio = 1,   // مراجعة عينات عملاء (تحتاج شيت إكسيل)
        VaultCashCount = 2,    // مراجعة وجرد الخزينة
        BranchOperations = 3,  // مراجعة العمليات والتشغيل
        SecurityAndSafety = 4  // مراجعة الأمن والسلامة
    }
}
