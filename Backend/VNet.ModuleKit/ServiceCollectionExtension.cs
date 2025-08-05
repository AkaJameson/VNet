using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Text.Json;
using VNet.ModuleKit.Core;
using VNet.ModuleKit.Entitys;
using VNet.ModuleKit.Localizer;

namespace VNet.ModuleKit
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 添加包
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        public static void AddPackages(this WebApplicationBuilder builder, Action<PackOptions> options)
        {
            var packOptions = new PackOptions();
            options(packOptions);
            PackageManager finder = new PackageManager(packOptions);
            builder.Services.AddSingleton(finder);
            foreach (var package in finder.GetPacks())
            {
                PackLoader.LoadPack(builder, package);
            }
            builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json")
                    , optional: false, reloadOnChange: true);
            foreach (var config in PackLoader.GetPackageConfiguration())
            {
                try
                {
                    using var stream = File.OpenRead(config);
                    using var doc = JsonDocument.Parse(stream);
                }
                catch
                {
                    continue;
                }
                builder.Configuration.AddJsonFile(config, false, true);
            }
            if (packOptions.EnableLocaizor)
            {
                builder.Services.AddModuleLocalization();
            }
        }

        /// <summary>
        /// 注册包
        /// </summary>
        /// <param name="app"></param>
        /// <param name="routes"></param>
        /// <param name="provider"></param>
        public static void UsePackages(this IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider provider, Action<RequestLocalizationOptions> localizorSetup = null)
        {
            var packManager = app.ApplicationServices.GetRequiredService<PackageManager>();
            if (packManager == null)
            {
                throw new ArgumentNullException(nameof(packManager));
            }
            if (packManager.GetPackOptions().EnableLocaizor)
            {
                UseModuleLocalization(app, packManager, localizorSetup);
            }
            PackLoader.UsePack(app, provider, routes);
        }
        /// <summary>
        /// 添加模块化本地化服务
        /// </summary>
        internal static IServiceCollection AddModuleLocalization(this IServiceCollection services)
        {
            services.AddSingleton<IStringLocalizerFactory, ModuleStringLocalizerFactory>();
            services.AddScoped(typeof(IStringLocalizer<>), typeof(ModuleStringLocalizer<>));
            services.AddLocalization();
            return services;
        }
        /// <summary>
        /// 使用模块化本地化
        /// </summary>
        internal static IApplicationBuilder UseModuleLocalization(IApplicationBuilder app, PackageManager packageManager, Action<RequestLocalizationOptions> setupAction = null)
        {
            if (packageManager != null)
            {
                var modules = packageManager.GetPacks();
                foreach (var module in modules)
                {
                    LocalizationResourceManager.LoadModuleResources(module);
                }
            }
            string rootPath = AppContext.BaseDirectory;
            LocalizationResourceManager.LoadSharedResources(rootPath);
            var options = new RequestLocalizationOptions()
                .AddSupportedCultures("zh-CN", "en-US")
                .AddSupportedUICultures("zh-CN", "en-US")
                .SetDefaultCulture("zh-CN");
            setupAction?.Invoke(options);
            return app.UseRequestLocalization(options);
        }
    }
}
