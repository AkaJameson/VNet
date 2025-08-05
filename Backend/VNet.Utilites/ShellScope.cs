using Microsoft.Extensions.DependencyInjection;

namespace VNet.Utilites
{
    /// <summary>
    /// 全局服务定位器，提供在非依赖注入上下文中访问服务的能力
    /// 警告：尽量避免过度使用此模式，优先考虑依赖注入
    /// </summary>
    public static class ShellScope
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        private static IServiceProvider _serviceProvider;

        /// <summary>
        /// 用于同步的对象
        /// </summary>
        private static readonly object _syncLock = new object();

        /// <summary>
        /// 是否已初始化
        /// </summary>
        private static bool _initialized;

        /// <summary>
        /// 创建一个服务范围
        /// </summary>
        /// <returns>服务范围</returns>
        public static IServiceScope CreateScope()
        {
            EnsureInitialized();
            return _serviceProvider.CreateScope();
        }

        /// <summary>
        /// 获取或设置服务提供者
        /// </summary>
        public static IServiceProvider ServiceProvider
        {
            get
            {
                EnsureInitialized();
                return _serviceProvider;
            }
            set
            {
                lock (_syncLock)
                {
                    _serviceProvider = value ?? throw new ArgumentNullException(nameof(value));
                    _initialized = true;
                }
            }
        }

        /// <summary>
        /// 注册服务定位器
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="configuration">配置（可选）</param>
        public static void RegisterShellScope(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            lock (_syncLock)
            {
                _serviceProvider = serviceProvider;
                _initialized = true;
            }
        }

        /// <summary>
        /// 确保已初始化
        /// </summary>
        /// <exception cref="InvalidOperationException">如果未初始化</exception>
        private static void EnsureInitialized()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("ShellScope未初始化，请先调用RegisterShellScope方法");
            }
        }
    }
}
