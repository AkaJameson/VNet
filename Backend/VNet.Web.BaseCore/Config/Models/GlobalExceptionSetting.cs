namespace VNet.Web.BaseCore.Config.Models
{
    /// <summary>
    /// 全局异常配置
    /// </summary>
    public class GlobalExceptionSetting
    {
        public bool IsEnable { get; set; } = true;
        public bool ShowDetail { get; set; } = false;
    }
}
