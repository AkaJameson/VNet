using System.Xml.Linq;
using VNet.Web.BaseCore.Config.Models;

namespace VNet.Web.BaseCore.Config
{
    /// <summary>
    /// 应用配置解析器
    /// </summary>
    public class AppConfigParser
    {
        private readonly string _configFilePath;

        public AppConfigParser(string configFilePath = "Data/appConfig.xml")
        {
            _configFilePath = configFilePath;
        }

        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <returns>应用配置对象</returns>
        public AppConfig Parse()
        {
            if (!File.Exists(_configFilePath))
                throw new FileNotFoundException($"配置文件不存在: {_configFilePath}");

            try
            {
                var doc = XDocument.Load(_configFilePath);
                var root = doc.Root ?? throw new InvalidOperationException("XML根元素不存在");

                var config = new AppConfig();

                // 解析数据库配置
                config.DbSetting = ParseDbSetting(root.Element("DbSetting"));

                // 解析缓存配置
                config.CacheSetting = ParseCacheSetting(root.Element("CacheSetting"));

                // 解析跨域配置
                config.AllowOrgins = root.Element("AllowOrgins")?.Attribute("value")?.Value ?? string.Empty;

                // 解析全局异常配置
                config.GlobalException = ParseGlobalExceptionSetting(root.Element("GlobalException"));

                // 解析Swagger配置
                config.Swagger = ParseSwaggerSetting(root.Element("Swagger"));

                // 解析邮件配置
                config.MailSetting = ParseMailSetting(root.Element("MailSetting"));

                // 解析短信配置
                config.SmsSetting = ParseSmsSetting(root.Element("SmsSetting"));

                // 解析上传配置
                config.UploadSetting = ParseUploadSetting(root.Element("UploadSetting"));

                return config;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"解析配置文件失败: {ex.Message}", ex);
            }
        }

        private DbSetting ParseDbSetting(XElement? dbElement)
        {
            var dbSetting = new DbSetting();
            if (dbElement == null) return dbSetting;

            // 解析MySQL配置
            var mysqlElement = dbElement.Element("Mysql");
            if (mysqlElement != null)
            {
                dbSetting.Mysql = new MysqlConfig
                {
                    IsEnable = GetElementValue<bool>(mysqlElement.Element("IsEnable")),
                    Host = GetElementValue(mysqlElement.Element("Host"), "localhost"),
                    Port = GetElementValue(mysqlElement.Element("Port"), 3306),
                    Database = GetElementValue<string>(mysqlElement.Element("Database")),
                    User = GetElementValue<string>(mysqlElement.Element("User")),
                    Password = GetElementValue<string>(mysqlElement.Element("Password"))
                };
            }

            // 解析SQLite配置
            var sqliteElement = dbElement.Element("Sqlite");
            if (sqliteElement != null)
            {
                dbSetting.Sqlite = new SqliteConfig
                {
                    IsEnable = GetElementValue<bool>(sqliteElement.Element("IsEnable")),
                    DbPath = GetElementValue<string>(sqliteElement.Element("DbPath"))
                };
            }

            // 解析SQL Server配置
            var sqlServerElement = dbElement.Element("SqlServer");
            if (sqlServerElement != null)
            {
                dbSetting.SqlServer = new SqlServerConfig
                {
                    IsEnable = GetElementValue<bool>(sqlServerElement.Element("IsEnable")),
                    Host = GetElementValue(sqlServerElement.Element("Host"), "localhost"),
                    Port = GetElementValue(sqlServerElement.Element("Port"), 1433),
                    Database = GetElementValue<string>(sqlServerElement.Element("Database")),
                    User = GetElementValue<string>(sqlServerElement.Element("User")),
                    Password = GetElementValue<string>(sqlServerElement.Element("Password"))
                };
            }

            // 解析Oracle配置
            var oracleElement = dbElement.Element("Oracle");
            if (oracleElement != null)
            {
                dbSetting.Oracle = new OracleConfig
                {
                    IsEnable = GetElementValue<bool>(oracleElement.Element("IsEnable")),
                    Host = GetElementValue(oracleElement.Element("Host"), "localhost"),
                    Port = GetElementValue(oracleElement.Element("Port"), 1521),
                    Database = GetElementValue<string>(oracleElement.Element("Database")),
                    User = GetElementValue<string>(oracleElement.Element("User")),
                    Password = GetElementValue<string>(oracleElement.Element("Password"))
                };
            }

            // 解析PostgreSQL配置
            var postgreElement = dbElement.Element("Postgre");
            if (postgreElement != null)
            {
                dbSetting.Postgre = new PostgreConfig
                {
                    IsEnable = GetElementValue<bool>(postgreElement.Element("IsEnable")),
                    Host = GetElementValue(postgreElement.Element("Host"), "localhost"),
                    Port = GetElementValue(postgreElement.Element("Port"), 5432),
                    Database = GetElementValue<string>(postgreElement.Element("Database")),
                    User = GetElementValue<string>(postgreElement.Element("User")),
                    Password = GetElementValue<string>(postgreElement.Element("Password"))
                };
            }

            return dbSetting;
        }

        private CacheSetting ParseCacheSetting(XElement? cacheElement)
        {
            var cacheSetting = new CacheSetting();
            if (cacheElement == null) return cacheSetting;

            var redisElement = cacheElement.Element("Redis");
            if (redisElement != null)
            {
                cacheSetting.Redis = new RedisConfig
                {
                    IsEnable = GetElementValue<bool>(redisElement.Element("IsEnable")),
                    Host = GetElementValue(redisElement.Element("Host"), "localhost"),
                    Port = GetElementValue(redisElement.Element("Port"), 6379),
                    Password = GetElementValue<string>(redisElement.Element("Password"))
                };
            }

            return cacheSetting;
        }

        private GlobalExceptionSetting ParseGlobalExceptionSetting(XElement? element)
        {
            var setting = new GlobalExceptionSetting();
            if (element == null) return setting;

            setting.IsEnable = GetElementValue(element.Element("IsEnable"), true);
            setting.ShowDetail = GetElementValue(element.Element("ShowDetail"), false);

            return setting;
        }

        private SwaggerSetting ParseSwaggerSetting(XElement? element)
        {
            var setting = new SwaggerSetting();
            if (element == null) return setting;

            setting.IsEnable = GetElementValue(element.Element("IsEnable"), true);
            setting.Title = GetElementValue(element.Element("Title"), "API");
            setting.Version = GetElementValue(element.Element("Version"), "v1");
            setting.Description = GetElementValue<string>(element.Element("Description"));

            return setting;
        }

        private MailSetting ParseMailSetting(XElement? element)
        {
            var setting = new MailSetting();
            if (element == null) return setting;

            setting.IsEnable = GetElementValue<bool>(element.Element("IsEnable"));
            setting.SmtpServer = GetElementValue<string>(element.Element("SmtpServer"));
            setting.Port = GetElementValue(element.Element("Port"), 587);
            setting.UseSsl = GetElementValue(element.Element("UseSsl"), true);
            setting.User = GetElementValue<string>(element.Element("User"));
            setting.Password = GetElementValue<string>(element.Element("Password"));

            return setting;
        }

        private SmsSetting ParseSmsSetting(XElement? element)
        {
            var setting = new SmsSetting();
            if (element == null) return setting;

            setting.Provider = GetElementValue<string>(element.Element("Provider"));
            setting.AccessKeyId = GetElementValue<string>(element.Element("AccessKeyId"));
            setting.AccessSecret = GetElementValue<string>(element.Element("AccessSecret"));
            setting.TemplateCode = GetElementValue<string>(element.Element("TemplateCode"));

            return setting;
        }

        private UploadSetting ParseUploadSetting(XElement? element)
        {
            var setting = new UploadSetting();
            if (element == null) return setting;

            setting.SavePath = GetElementValue(element.Element("SavePath"), "wwwroot/uploads");
            setting.MaxFileSize = GetElementValue<long>(element.Element("MaxFileSize"), 10485760);
            setting.AllowedExtensions = GetElementValue(element.Element("AllowedExtensions"), ".jpg,.png,.pdf,.docx,.xlsx");

            return setting;
        }

        private T GetElementValue<T>(XElement? element, T defaultValue = default)
        {
            if (element == null || string.IsNullOrWhiteSpace(element.Value))
                return defaultValue;

            try
            {
                return (T)Convert.ChangeType(element.Value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }

}
