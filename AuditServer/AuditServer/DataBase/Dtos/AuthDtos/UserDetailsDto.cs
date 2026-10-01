namespace AuditServer.DataBase.Dtos.AuthDtos
{
    public class UserDetailsDto : UserListDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int DepartmentId { get; set; }

        // إرجاع قائمة المناطق المنسوبة للمراجع
        public List<string> RegionNames { get; set; } = new();
        public List<int> RegionIds { get; set; } = new();

        // إرجاع الأدوار
        public List<string> Roles { get; set; } = new();
    }
}
