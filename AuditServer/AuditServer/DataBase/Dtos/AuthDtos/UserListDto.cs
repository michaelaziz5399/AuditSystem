namespace AuditServer.DataBase.Dtos.AuthDtos
{
    // 1. DTO للقوائم (خفيف جداً)
    public class UserListDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
