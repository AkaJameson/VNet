namespace VNet.Web.BaseCore.Config.Models
{
    /// <summary>
    /// 应用程序配置根模型
    /// </summary>
    public class AppConfig
    {
        public DbSetting DbSetting { get; set; } = new DbSetting();
        public CacheSetting CacheSetting { get; set; } = new CacheSetting();
        public string AllowOrgins { get; set; } = string.Empty;
        public GlobalExceptionSetting GlobalException { get; set; } = new GlobalExceptionSetting();
        public SwaggerSetting Swagger { get; set; } = new SwaggerSetting();
        public MailSetting MailSetting { get; set; } = new MailSetting();
        public SmsSetting SmsSetting { get; set; } = new SmsSetting();
        public UploadSetting UploadSetting { get; set; } = new UploadSetting();
    }
}
