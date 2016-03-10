using System.Threading.Tasks;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace SocialGamificationAsset.Middlewares
{
    /// <summary>
    /// </summary>
    internal class NoServerHttpHeaderMiddleware
    {
        private const string ServerHttpHeaderName = "Server";

        private readonly RequestDelegate _next;

        /// <summary>
        /// </summary>
        /// <param name="next"></param>
        public NoServerHttpHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        /// </returns>
        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Remove(ServerHttpHeaderName);

            await _next.Invoke(context);
        }
    }
}