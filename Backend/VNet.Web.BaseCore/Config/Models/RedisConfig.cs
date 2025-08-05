namespace VNet.Web.BaseCore.Config.Models
{
    public class RedisConfig
    {
        public bool IsEnable { get; set; }
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 6379;
        public string Password { get; set; } = string.Empty;

        public string GetConnectionString()
        {
            var connectionString = $"{Host}:{Port}";
            if (!string.IsNullOrEmpty(Password))
            {
                connectionString += $",password={Password}";
            }
            return connectionString;
        }
    }
}
