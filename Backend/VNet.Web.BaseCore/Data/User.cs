namespace VNet.Web.BaseCore.Data
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public DateTime? LastLoginAt { get; set; }
        public string? LastLoginIp { get; set; }
        public int LoginFailCount { get; set; } = 0;
        public DateTime? LockUntil { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public UserProfile? Profile { get; set; }
    }
}
