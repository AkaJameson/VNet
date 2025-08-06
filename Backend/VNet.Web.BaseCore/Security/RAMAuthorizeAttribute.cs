using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace VNet.Web.BaseCore.Security
{
    public class RAMAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _permissionCode;
        public RAMAuthorizeAttribute(string permissionCode)
        {
            _permissionCode = permissionCode;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var service = context.HttpContext.RequestServices;
            var permissionManager = service.GetRequiredService<IPermissionManager>();
            var user = context.HttpContext.User;

        }
    }
}
