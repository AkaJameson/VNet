using VNet.Web.BaseCore.Config.Abstraction;

namespace VNet.Web.BaseCore.Config.Models
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DbSetting
    {
        public MysqlConfig Mysql { get; set; } = new MysqlConfig();
        public SqliteConfig Sqlite { get; set; } = new SqliteConfig();
        public SqlServerConfig SqlServer { get; set; } = new SqlServerConfig();
        public OracleConfig Oracle { get; set; } = new OracleConfig();
        public PostgreConfig Postgre { get; set; } = new PostgreConfig();

        /// <summary>
        /// 获取当前启用的数据库配置
        /// </summary>
        public IDbConfig GetActiveDbConfig()
        {
            if (Mysql.IsEnable) return Mysql;
            if (Sqlite.IsEnable) return Sqlite;
            if (SqlServer.IsEnable) return SqlServer;
            if (Oracle.IsEnable) return Oracle;
            if (Postgre.IsEnable) return Postgre;

            throw new InvalidOperationException("没有启用的数据库配置");
        }
    }
}
