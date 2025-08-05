namespace VNet.Web.BaseCore.Config.Models
{
    public class CacheSetting
    {
        public RedisConfig Redis { get; set; } = new RedisConfig();
    }
}
