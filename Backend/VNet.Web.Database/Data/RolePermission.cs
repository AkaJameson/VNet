namespace VNet.Web.Database.Data
{
    /// <summary>
    /// 角色权限关联表
    /// </summary>
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}
