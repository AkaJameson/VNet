namespace VNet.Web.Database.Data
{
    /// <summary>
    /// API接口表
    /// </summary>
    public class ApiEndpoint
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty; // GET, POST, PUT, DELETE
        public string Path { get; set; } = string.Empty;
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool RequireAuth { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<PermissionApiEndpoint> PermissionApiEndpoints { get; set; } = new List<PermissionApiEndpoint>();
    }
}
