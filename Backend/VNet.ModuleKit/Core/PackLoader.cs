using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using VNet.ModuleKit.Entitys;
using VNet.Utilites;

namespace VNet.ModuleKit.Core
{
    /// <summary>
    /// 模块加载器，负责加载模块和管理模块的生命周期
    /// </summary>
    public static class PackLoader
    {
        /// <summary>
        /// 已加载的模块实例集合
        /// </summary>
        private static readonly ConcurrentDictionary<string, PackBase> _packInstances = new ConcurrentDictionary<string, PackBase>();
        private static readonly ConcurrentDictionary<string, string> _packConfigurations = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 加载模块到应用程序
        /// </summary>
        /// <param name="builder">Web应用程序构建器</param>
        /// <param name="moduleInfo">模块信息</param>
        public static void LoadPack(WebApplicationBuilder builder, ModuleInfo moduleInfo)
        {
            using var sp = ShellScope.CreateScope();
            var logger = sp.ServiceProvider.GetRequiredService<ILogger>();
            if (moduleInfo?.Assembly == null)
            {
                logger.LogWarning("尝试加载无效的模块");
                return;
            }

            try
            {
                // 查找模块中继承自PackBase的类型
                var packType = moduleInfo.Assembly.GetTypes()
                    .FirstOrDefault(t => !t.IsAbstract && typeof(PackBase).IsAssignableFrom(t));

                if (packType == null)
                {
                    logger.LogWarning($"模块 {moduleInfo.AssemblyName} 中未找到继承自PackBase的类型");
                    return;
                }

                // 创建模块实例
                var packInstance = (PackBase)Activator.CreateInstance(packType);
                if (packInstance == null)
                {
                    logger.LogError($"无法创建模块 {moduleInfo.AssemblyName} 的实例");
                    return;
                }

                // 初始化模块
                packInstance.Initialize();

                // 将模块实例添加到集合中
                string moduleName = moduleInfo.Assembly.GetName().Name;
                _packInstances.TryAdd(moduleName, packInstance);

                // 注册模块程序集到MVC
                var partManager = builder.Services.AddMvcCore();
                partManager.AddApplicationPart(moduleInfo.Assembly);

                // 加载模块配置
                LoadModuleConfiguration(moduleInfo);

                // 调用模块的服务配置方法
                packInstance.ConfigurationServices(builder, builder.Services);

                logger.LogInformation($"成功加载模块 {moduleInfo.AssemblyName}");

                moduleInfo.IsLoaded = true;
            }
            catch (Exception ex)
            {
                logger.LogError($"加载模块 {moduleInfo.AssemblyName} 失败: {ex.Message}\n{ex.StackTrace}");
                moduleInfo.IsLoaded = false;
            }
        }

        /// <summary>
        /// 加载模块配置
        /// </summary>
        /// <param name="moduleInfo">模块信息</param>
        private static void LoadModuleConfiguration(ModuleInfo moduleInfo)
        {
            using var sp = ShellScope.CreateScope();
            var logger = sp.ServiceProvider.GetRequiredService<ILogger>();
            try
            {
                string moduleName = moduleInfo.Assembly.GetName().Name;
                // 注册配置
                RegisterConfiguration(moduleName, moduleInfo.ConfigFile);
                logger.LogInformation($"加载模块 {moduleInfo.AssemblyName} 的配置文件成功");
            }
            catch (Exception ex)
            {
                logger.LogError($"加载模块 {moduleInfo.AssemblyName} 的配置文件失败: {ex.ToString()}");
                throw;
            }
        }

        /// <summary>
        /// 使用模块中间件和路由
        /// </summary>
        /// <param name="app">应用程序构建器</param>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="routes">路由构建器</param>
        public static void UsePack(IApplicationBuilder app, IServiceProvider serviceProvider, IEndpointRouteBuilder routes)
        {
            using var sp = ShellScope.CreateScope();
            var logger = sp.ServiceProvider.GetRequiredService<ILogger>();
            foreach (var packInstance in _packInstances.Values)
            {
                try
                {
                    packInstance.Configuration(app, routes, serviceProvider);
                    logger.LogInformation($"配置模块 {packInstance.Name} 成功");
                }
                catch (Exception ex)
                {
                    logger.LogError($"配置模块 {packInstance.Name} 失败: {ex.ToString()}");
                }
            }

            // 启动所有模块
            foreach (var packInstance in _packInstances.Values)
            {
                try
                {
                    packInstance.Startup(serviceProvider);
                    logger.LogInformation($"启动模块 {packInstance.Name} 成功");
                }
                catch (Exception ex)
                {
                    logger.LogError($"启动模块 {packInstance.Name} 失败: {ex.ToString()}");
                }
            }
        }

        /// <summary>
        /// 注册模块配置
        /// </summary>
        /// <param name="packName">模块名称</param>
        /// <param name="configuration">配置</param>
        public static void RegisterConfiguration(string packName, string configurationPath)
        {
            if (string.IsNullOrEmpty(packName) || string.IsNullOrEmpty(configurationPath) || File.Exists(configurationPath))
            {
                return;
            }
            _packConfigurations.TryAdd(packName, configurationPath);
        }

        /// <summary>
        /// 获取已加载模块实例
        /// </summary>
        /// <param name="packName">模块名称</param>
        /// <returns>模块实例，未找到返回null</returns>
        public static PackBase GetPackInstance(string packName)
        {
            if (string.IsNullOrEmpty(packName))
            {
                return null;
            }

            _packInstances.TryGetValue(packName, out PackBase packInstance);
            return packInstance;
        }

        public static List<string> GetPackageConfiguration()
        {
            return _packConfigurations.Values.Distinct().ToList();
        }

        /// <summary>
        /// 获取所有已加载的模块实例
        /// </summary>
        /// <returns>模块实例集合</returns>
        public static IEnumerable<PackBase> GetAllPackInstances()
        {
            return _packInstances.Values;
        }

        /// <summary>
        /// 关闭所有模块
        /// </summary>
        public static void ShutdownAllPacks()
        {
            foreach (var packInstance in _packInstances.Values)
            {
                packInstance.Shutdown();
            }
            _packInstances.Clear();
            _packConfigurations.Clear();
        }
    }
}
