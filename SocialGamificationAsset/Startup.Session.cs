using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;

using SocialGamificationAsset.Middlewares;

namespace SocialGamificationAsset
{
    public partial class Startup
    {
        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureSessionServices(IServiceCollection services)
        {
            services.AddSession();

            // Configure Authentication filters
            services.AddSingleton<ISessionAuthorizeFilter, SessionAuthorizeFilter>();
        }

        /// <summary>
        /// </summary>
        /// <param name="application"></param>
        private static void ConfigureSession(IApplicationBuilder application)
        {
            application.UseSession();
        }
    }
}