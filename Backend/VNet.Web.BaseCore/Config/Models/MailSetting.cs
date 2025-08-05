namespace VNet.Web.BaseCore.Config.Models
{
    /// <summary>
    /// 邮件配置
    /// </summary>
    public class MailSetting
    {
        public bool IsEnable { get; set; }
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
