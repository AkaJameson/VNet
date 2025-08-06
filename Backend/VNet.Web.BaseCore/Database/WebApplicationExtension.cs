using Microsoft.Extensions.DependencyInjection;
using VNet.Web.BaseCore.Config;

namespace VNet.Web.BaseCore.Database
{
    public static class WebApplicationExtension
    {
        public static IServiceCollection AddVNetDbContext<T>(this IServiceCollection services) where T : VNetDbContext
        {
            return services.AddConfiguredDatabase<T>();
        }
    }
}
