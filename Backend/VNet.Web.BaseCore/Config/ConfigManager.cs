using VNet.Web.BaseCore.Config.Models;

namespace VNet.Web.BaseCore.Config
{
    /// <summary>
    /// 配置管理器 - 单例模式
    /// </summary>
    public class ConfigManager
    {
        private static readonly Lazy<ConfigManager> _instance = new(() => new ConfigManager());
        private AppConfig? _config;
        private readonly object _lock = new object();

        public static ConfigManager Instance => _instance.Value;

        private ConfigManager() { }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        public void LoadConfig(string configPath = "Data/appConfig.xml")
        {
            lock (_lock)
            {
                var parser = new AppConfigParser(configPath);
                _config = parser.Parse();
            }
        }

        /// <summary>
        /// 获取完整配置
        /// </summary>
        public AppConfig GetConfig()
        {
            if (_config == null)
                throw new InvalidOperationException("配置未加载，请先调用LoadConfig方法");

            return _config;
        }

        /// <summary>
        /// 获取数据库配置
        /// </summary>
        public DbSetting GetDbConfig() => GetConfig().DbSetting;

        /// <summary>
        /// 获取缓存配置
        /// </summary>
        public CacheSetting GetCacheConfig() => GetConfig().CacheSetting;

        /// <summary>
        /// 获取跨域配置
        /// </summary>
        public string[] GetAllowOrigins()
        {
            var allowOrigins = GetConfig().AllowOrgins;
            return string.IsNullOrEmpty(allowOrigins)
                ? Array.Empty<string>()
                : allowOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(s => s.Trim())
                             .ToArray();
        }

        /// <summary>
        /// 获取全局异常配置
        /// </summary>
        public GlobalExceptionSetting GetGlobalExceptionConfig() => GetConfig().GlobalException;

        /// <summary>
        /// 获取Swagger配置
        /// </summary>
        public SwaggerSetting GetSwaggerConfig() => GetConfig().Swagger;

        /// <summary>
        /// 获取邮件配置
        /// </summary>
        public MailSetting GetMailConfig() => GetConfig().MailSetting;

        /// <summary>
        /// 获取短信配置
        /// </summary>
        public SmsSetting GetSmsConfig() => GetConfig().SmsSetting;

        /// <summary>
        /// 获取上传配置
        /// </summary>
        public UploadSetting GetUploadConfig() => GetConfig().UploadSetting;
    }
}
