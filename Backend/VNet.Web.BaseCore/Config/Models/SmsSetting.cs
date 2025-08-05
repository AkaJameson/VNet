namespace VNet.Web.BaseCore.Config.Models
{
    /// <summary>
    /// 短信配置
    /// </summary>
    public class SmsSetting
    {
        public string Provider { get; set; } = string.Empty;
        public string AccessKeyId { get; set; } = string.Empty;
        public string AccessSecret { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;
    }
}
