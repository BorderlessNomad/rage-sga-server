using System.Threading.Tasks;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace SocialGamificationAsset.Middlewares
{
    internal class NoServerHttpHeaderMiddleware
    {
        private const string ServerHttpHeaderName = "Server";

        private readonly RequestDelegate next;

        public NoServerHttpHeaderMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Remove(ServerHttpHeaderName);
            await next.Invoke(context);
        }
    }
}