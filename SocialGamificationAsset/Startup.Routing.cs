using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SocialGamificationAsset
{
    public partial class Startup
    {
        /// <summary>
        ///     Improve SEO by stopping duplicate URL's due to case differences or
        ///     trailing slashes. See
        ///     http://googlewebmastercentral.blogspot.co.uk/2010/04/to-slash-or-not-to-slash.html
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureRouting(IServiceCollection services)
        {
            // Add CORS support to the service
            services.AddCors(
                options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
        }

        /// <summary>
        /// </summary>
        /// <param name="application"></param>
        private static void ConfigureCors(IApplicationBuilder application)
        {
            application.UseCors("AllowAll");
        }
    }
}