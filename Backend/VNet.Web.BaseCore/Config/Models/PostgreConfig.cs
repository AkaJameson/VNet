using VNet.Web.BaseCore.Config.Abstraction;

namespace VNet.Web.BaseCore.Config.Models
{
    public class PostgreConfig : IDbConfig
    {
        public bool IsEnable { get; set; }
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5432;
        public string Database { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string GetConnectionString()
        {
            return $"Host={Host};Port={Port};Database={Database};Username={User};Password={Password};";
        }
    }
}
