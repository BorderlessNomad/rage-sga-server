using Microsoft.AspNet.Builder;

using SocialGamificationAsset.Middlewares;

namespace SocialGamificationAsset
{
    public partial class Startup
    {
        /// <summary>
        ///     Adds HTTP Header for Time take for request to execute on server.
        /// </summary>
        /// <param name="application">The application.</param>
        private static void ConfigureDiagnosticInformation(IApplicationBuilder application)
        {
            application.UseMiddleware<ResponseTimerMiddleware>();
        }
    }
}