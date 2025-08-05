namespace VNet.Web.Database.Data
{
    /// <summary>
    /// 权限表
    /// </summary>
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int Sort { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<PermissionApiEndpoint> PermissionApiEndpoints { get; set; } = new List<PermissionApiEndpoint>();
    }
}
