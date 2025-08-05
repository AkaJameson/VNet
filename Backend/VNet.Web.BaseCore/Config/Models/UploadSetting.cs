namespace VNet.Web.BaseCore.Config.Models
{
    /// <summary>
    /// 上传配置
    /// </summary>
    public class UploadSetting
    {
        public string SavePath { get; set; } = "wwwroot/uploads";
        public long MaxFileSize { get; set; } = 10485760; // 10MB
        public string AllowedExtensions { get; set; } = ".jpg,.png,.pdf,.docx,.xlsx";

        public List<string> GetAllowedExtensionsList()
        {
            return AllowedExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(ext => ext.Trim().ToLowerInvariant())
                                  .ToList();
        }

        public bool IsExtensionAllowed(string extension)
        {
            var allowedExts = GetAllowedExtensionsList();
            return allowedExts.Contains(extension.ToLowerInvariant());
        }
    }

}
