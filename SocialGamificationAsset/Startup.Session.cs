using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;

using SocialGamificationAsset.Middlewares;

namespace SocialGamificationAsset
{
    public partial class Startup
    {
        private static void ConfigureSessionServices(IServiceCollection services)
        {
            services.AddSession();

            // Configure Auth
            services.AddSingleton<ISessionAuthorizeFilter, SessionAuthorizeFilter>();
        }

        private static void ConfigureSession(IApplicationBuilder application)
        {
            application.UseSession();
        }
    }
}