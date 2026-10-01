namespace AuditServer.DataBase.Enums
{
    // 3. المخالفات المستندية والأوراق
    public enum DocumentaryViolation
    {
        IncompleteDocuments = 1,     // أوراق غير كاملة
        DocumentErasure = 2,         // يوجد شطب أو تعديل بالجهة
        MissingSignature = 3         // عدم استكمال التوقيعات
    }
}
