namespace VNet.Web.BaseCore.Data
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? RealName { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public int Gender { get; set; } = 0; // 0:未知 1:男 2:女
        public DateTime? Birthday { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public User User { get; set; }
    }
}
