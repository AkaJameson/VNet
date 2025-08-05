namespace VNet.Web.BaseCore.Config.Models
{
    /// <summary>
    /// Swagger配置
    /// </summary>
    public class SwaggerSetting
    {
        public bool IsEnable { get; set; } = true;
        public string Title { get; set; } = "API";
        public string Version { get; set; } = "v1";
        public string Description { get; set; } = string.Empty;
    }
}
