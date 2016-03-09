using Microsoft.AspNet.Builder;

using SocialGamificationAsset.Middlewares;

namespace SocialGamificationAsset
{
    public partial class Startup
    {
        private static void ConfigureDiagnosticInformation(IApplicationBuilder application)
        {
            application.UseMiddleware<ResponseTimerMiddleware>();
        }
    }
}