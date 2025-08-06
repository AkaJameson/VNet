using Microsoft.Extensions.Caching.Distributed;

namespace VNet.Utilites.Extension
{
    public static class CacheExtensions
    {
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public static async Task SetObjectAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan? expiration = null)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            });
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T?> GetObjectAsync<T>(this IDistributedCache cache, string key)
        {
            var json = await cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(json)) return default;
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }

}
