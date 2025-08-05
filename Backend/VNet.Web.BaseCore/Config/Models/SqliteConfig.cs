using VNet.Web.BaseCore.Config.Abstraction;

namespace VNet.Web.BaseCore.Config.Models
{
    public class SqliteConfig : IDbConfig
    {
        public bool IsEnable { get; set; }
        public string DbPath { get; set; } = string.Empty;

        public string GetConnectionString()
        {
            return $"Data Source={DbPath}";
        }
    }
}
