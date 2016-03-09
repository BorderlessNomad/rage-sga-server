using System.Threading.Tasks;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace SocialGamificationAsset.Middlewares
{
    internal class InternalServerErrorOnExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public InternalServerErrorOnExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}