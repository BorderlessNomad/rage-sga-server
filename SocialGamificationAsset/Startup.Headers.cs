using Microsoft.AspNet.Builder;

using SocialGamificationAsset.Middlewares;

namespace SocialGamificationAsset
{
    public partial class Startup
    {
        /// <summary>
        ///     Allow passing X-Http-Header-Override in Http Header for requests to
        ///     be treated as other than GET/DELETE.
        /// </summary>
        /// <param name="application">The application.</param>
        private static void ConfigureHeadersOverride(IApplicationBuilder application)
        {
            application.UseMiddleware<XHttpHeaderOverrideMiddleware>();
        }
    }
}