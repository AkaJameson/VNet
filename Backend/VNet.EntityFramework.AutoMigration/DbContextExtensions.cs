using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VNet.EntityFramework.AutoMigration.Configuration;
using VNet.EntityFramework.AutoMigration.Core;

namespace VNet.EntityFramework.AutoMigration
{
    /// <summary>
    /// 提供DbContext自动迁移功能的扩展方法
    /// </summary>
    public static class DbContextExtensions
    {
        public static void AddAutoMigrationProvider<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddScoped<DbContext, TContext>();
            services.AddScoped<MigrationExecuter>();
            services.AddScoped<MigrationStepProcessor>();
        }
        /// <summary>
        /// 同步检查并更新数据库表结构
        /// </summary>
        /// <param name="context">数据库上下文</param>
        /// <param name="options">迁移选项</param>
        internal static void AutoMigration(this DbContext context, IServiceProvider sp, AutoMigrationOptions options = null)
        {
            context.AutoMigrationAsync(sp, options).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 异步检查并更新数据库表结构
        /// </summary>
        /// <param name="context">数据库上下文</param>
        /// <param name="options">迁移选项</param>
        internal static async Task AutoMigrationAsync(this DbContext context, IServiceProvider sp, AutoMigrationOptions options = null)
        {
            options = options ?? new AutoMigrationOptions();
            var executer = sp.GetRequiredService<MigrationExecuter>();
            await executer.Migrate(context, options);
        }

        public static async Task AutoMigrationAsync<T>(this WebApplication app, AutoMigrationOptions options = null) where T : DbContext
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<T>();
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            await context.AutoMigrationAsync(scope.ServiceProvider, options);
        }
        public static void AutoMigration<T>(this WebApplication app, AutoMigrationOptions options = null) where T : DbContext
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<T>();
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            context.AutoMigration(scope.ServiceProvider, options);
        }
    }
}