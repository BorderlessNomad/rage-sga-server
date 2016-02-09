using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
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
        /// <param name="routeOptions">The routing options.</param>
        private static void ConfigureRouting(IServiceCollection services, RouteOptions routeOptions)
        {
            // All generated URL's should append a trailing slash.
            routeOptions.AppendTrailingSlash = true;

            // All generated URL's should be lower-case.
            routeOptions.LowercaseUrls = true;

            // TODO: IgnoreRoute does not yet exist in MVC 6.

            // // IgnoreRoute - Tell the routing system to ignore certain routes for better performance.
            // // Ignore .axd files.
            // routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            // // Ignore everything in the Content folder.
            // routes.IgnoreRoute("Content/{*pathInfo}");
            // // Ignore the humans.txt file.
            // routes.IgnoreRoute("humans.txt");

            // Add CORS support to the service
            services.AddCors(
                options =>
                    {
                        options.AddPolicy(
                            "AllowAll",
                            builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
                    });
        }

        private static void ConfigureCors(IApplicationBuilder application)
        {
            application.UseCors("AllowAll");
        }
    }
}