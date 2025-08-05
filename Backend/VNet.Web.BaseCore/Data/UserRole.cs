namespace VNet.Web.BaseCore.Data
{
    /// <summary>
    /// 用户角色关联表
    /// </summary>
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
