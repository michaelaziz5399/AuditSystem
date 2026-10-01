namespace AuditServer.DataBase.Enums
{
    // 1. مخالفات الزيارة والميدان
    public enum FieldVisitViolation
    {
        NoActivity = 1,              // عدم وجود نشاط
        ClosedActivity = 2,          // النشاط مغلق
        BadReputation = 3,           // سمعة العميل سيئة
        ActivityNotForCustomer = 4,  // النشاط لا يخص العميل
        DisbursedToRelativity = 5,   // صرف الأخصائي لأحد أقاربه
        SplitLoanWithGuarantor = 6,  // قسمة قرض بين العميل والضامن
        CustomerNotBenefiting = 7    // عميل غير مستفيد بالقرض
    }
}
