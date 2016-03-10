using System;
using System.Threading.Tasks;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

using Serilog;

namespace SocialGamificationAsset.Middlewares
{
    /// <summary>
    /// </summary>
    internal class InternalServerErrorOnExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// </summary>
        /// <param name="next"></param>
        public InternalServerErrorOnExceptionMiddleware(RequestDelegate next)
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
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}