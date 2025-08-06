using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using VNet.Utilites.Extension;
using VNet.Web.BaseCore.Config;
using VNet.Web.BaseCore.Database;

namespace VNet.Web.BaseCore.Security
{
    public class PermissionManager : IPermissionManager
    {
        private readonly VNetDbContext vNetDbContext;
        private readonly IMemoryCache? memoryCache;
        private readonly IDistributedCache? distributedCache;
        private readonly bool isRedisEnabled;
        private readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

        public PermissionManager(VNetDbContext vNetDbContext,
                                 IServiceProvider provider)
        {
            this.vNetDbContext = vNetDbContext;
            isRedisEnabled = ConfigManager.Instance.GetCacheConfig().Redis.IsEnable;
            if (isRedisEnabled)
            {
                distributedCache = provider.GetRequiredService<IDistributedCache>();
            }
            else
            {
                memoryCache = provider.GetRequiredService<IMemoryCache>();
            }
        }

        /// <summary>
        /// 获取用户权限（缓存）
        /// </summary>
        public async Task<HashSet<string>> GetPermissionByUserIdAsync(int userId)
        {
            string cacheKey = $"Permission_{userId}";

            if (isRedisEnabled && distributedCache != null)
            {
                var cached = await distributedCache.GetObjectAsync<HashSet<string>>(cacheKey);
                if (cached != null) return cached;
            }
            else if (memoryCache != null && memoryCache.TryGetValue(cacheKey, out HashSet<string> memPermissions))
            {
                return memPermissions!;
            }

            // 缓存不存在 → 查询数据库
            var permissionList = await (from ur in vNetDbContext.UserRoles
                                        join rp in vNetDbContext.RolePermissions on ur.RoleId equals rp.RoleId
                                        join p in vNetDbContext.Permissions on rp.PermissionId equals p.Id
                                        where ur.UserId == userId && p.IsActive
                                        select p.Code).Distinct().ToListAsync();

            var permissionSet = new HashSet<string>(permissionList);

            // 写入缓存
            if (isRedisEnabled && distributedCache != null)
            {
                await distributedCache.SetObjectAsync(cacheKey, permissionSet, CacheExpiration);
            }
            else if (memoryCache != null)
            {
                memoryCache.Set(cacheKey, permissionSet, CacheExpiration);
            }

            return permissionSet;
        }

        /// <summary>
        /// 刷新用户权限缓存
        /// </summary>
        public async Task<HashSet<string>> RefreshPermissionCacheAsync(int userId)
        {
            string cacheKey = $"Permission_{userId}";

            var permissionList = await (from ur in vNetDbContext.UserRoles
                                        join rp in vNetDbContext.RolePermissions on ur.RoleId equals rp.RoleId
                                        join p in vNetDbContext.Permissions on rp.PermissionId equals p.Id
                                        where ur.UserId == userId && p.IsActive
                                        select p.Code).Distinct().ToListAsync();

            var permissionSet = new HashSet<string>(permissionList);

            // 强制更新缓存
            if (isRedisEnabled && distributedCache != null)
            {
                await distributedCache.SetObjectAsync(cacheKey, permissionSet, CacheExpiration);
            }
            else if (memoryCache != null)
            {
                memoryCache.Set(cacheKey, permissionSet, CacheExpiration);
            }

            return permissionSet;
        }
    }
}
