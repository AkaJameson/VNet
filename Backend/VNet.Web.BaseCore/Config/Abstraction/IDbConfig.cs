namespace VNet.Web.BaseCore.Config.Abstraction
{
    /// <summary>
    /// 数据库配置接口
    /// </summary>
    public interface IDbConfig
    {
        bool IsEnable { get; set; }
        string GetConnectionString();
    }

}
