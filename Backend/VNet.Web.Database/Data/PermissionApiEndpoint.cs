namespace VNet.Web.Database.Data
{
    /// <summary>
    /// 权限接口关联表
    /// </summary>
    public class PermissionApiEndpoint
    {
        public int PermissionId { get; set; }
        public int ApiEndpointId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Permission Permission { get; set; } = null!;
        public ApiEndpoint ApiEndpoint { get; set; } = null!;
    }
}
