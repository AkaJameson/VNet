using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace VNet.Web.BaseCore.Security
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, Session session)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClain = context.User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? context.User.FindFirst("sub")
                    ?? context.User.FindFirst("userId");

            }
        }
    }
}
