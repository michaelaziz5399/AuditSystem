namespace AuditServer.DataBase.Enums
{
    // 2. المخالفات المالية والتوريدات
    public enum FinancialViolation
    {
        NonInstallmentPayment = 1,   // عدم توريد القسط
        DelayedInstallmentPayment = 2, // تأخير توريد القسط
        OfficerCustomerSplit = 3     // قسمة الأخصائي مع العميل
    }
}
