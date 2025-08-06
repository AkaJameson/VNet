
namespace VNet.Web.BaseCore.Security
{
    public interface IPermissionManager
    {
        Task<HashSet<string>> GetPermissionByUserIdAsync(int userId);
        Task<HashSet<string>> RefreshPermissionCacheAsync(int userId);
    }
}