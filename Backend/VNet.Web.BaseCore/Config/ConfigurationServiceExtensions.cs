using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VNet.Web.BaseCore.Config.Models;

namespace VNet.Web.BaseCore.Config
{
    /// <summary>
    /// 配置服务扩展
    /// </summary>
    public static class ConfigurationServiceExtensions
    {
        /// <summary>
        /// 添加XML配置服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configPath">配置文件路径</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddXmlConfiguration(this IServiceCollection services, string configPath = "Data/appConfig.xml")
        {
            // 加载配置
            ConfigManager.Instance.LoadConfig(configPath);
            var appConfig = ConfigManager.Instance.GetConfig();

            // 注册配置对象为单例
            services.AddSingleton(appConfig);
            services.AddSingleton(appConfig.DbSetting);
            services.AddSingleton(appConfig.CacheSetting);
            services.AddSingleton(appConfig.GlobalException);
            services.AddSingleton(appConfig.Swagger);
            services.AddSingleton(appConfig.MailSetting);
            services.AddSingleton(appConfig.SmsSetting);
            services.AddSingleton(appConfig.UploadSetting);

            // 注册配置管理器
            services.AddSingleton(ConfigManager.Instance);

            return services;
        }

        /// <summary>
        /// 添加CORS配置
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="policyName">策略名称</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddConfiguredCors(this IServiceCollection services, string policyName = "DefaultPolicy")
        {
            var allowOrigins = ConfigManager.Instance.GetAllowOrigins();

            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    if (allowOrigins.Length > 0)
                    {
                        builder.WithOrigins(allowOrigins);
                    }
                    else
                    {
                        builder.AllowAnyOrigin();
                    }

                    builder.AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            return services;
        }

        /// <summary>
        /// 添加数据库连接配置
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddConfiguredDatabase<T>(this IServiceCollection services) where T : DbContext
        {
            var dbConfig = ConfigManager.Instance.GetDbConfig();
            var activeConfig = dbConfig.GetActiveDbConfig();

            switch (activeConfig)
            {
                case MysqlConfig mysql:
                    services.AddDbContext<T>(options =>
                        options.UseMySql(mysql.GetConnectionString(), ServerVersion.AutoDetect(mysql.GetConnectionString())));
                    break;

                case SqliteConfig sqlite:
                    services.AddDbContext<T>(options =>
                        options.UseSqlite(sqlite.GetConnectionString()));
                    break;

                case SqlServerConfig sqlServer:
                    services.AddDbContext<T>(options =>
                        options.UseSqlServer(sqlServer.GetConnectionString()));
                    break;

                case OracleConfig oracle:
                    //services.AddDbContext<YourDbContext>(options =>
                    //    options.UseOracle(oracle.GetConnectionString()));
                    break;

                case PostgreConfig postgre:
                    services.AddDbContext<T>(options =>
                        options.UseNpgsql(postgre.GetConnectionString()));
                    break;
            }

            return services;
        }

        /// <summary>
        /// 添加Redis缓存配置
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddConfiguredRedis(this IServiceCollection services)
        {
            var cacheConfig = ConfigManager.Instance.GetCacheConfig();

            if (cacheConfig.Redis.IsEnable)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = cacheConfig.Redis.GetConnectionString();
                });
            }
            else
            {
                services.AddMemoryCache();
            }

            return services;
        }
    }
}
