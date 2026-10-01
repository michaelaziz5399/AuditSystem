namespace AuditServer.DataBase.Entities
{
    public class UserRegion
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int RegionId { get; set; }
        public Region Region { get; set; } = null!;
    }
}
