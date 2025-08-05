using VNet.Web.BaseCore.Config.Abstraction;

namespace VNet.Web.BaseCore.Config.Models
{
    public class SqlServerConfig : IDbConfig
    {
        public bool IsEnable { get; set; }
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 1433;
        public string Database { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string GetConnectionString()
        {
            return $"Server={Host},{Port};Database={Database};User Id={User};Password={Password};";
        }
    }
}
