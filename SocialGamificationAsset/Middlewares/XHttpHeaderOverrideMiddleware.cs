using System;
using System.Threading.Tasks;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace SocialGamificationAsset.Middlewares
{
    internal class XHttpHeaderOverrideMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// </summary>
        /// <param name="next"></param>
        public XHttpHeaderOverrideMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <exception cref="Exception">
        ///     A <see langword="delegate" /> callback throws an exception.
        /// </exception>
        public async Task Invoke(HttpContext context)
        {
            var headerValue = context.Request.Headers["X-HTTP-Method-Override"];
            var queryValue = context.Request.Query["X-HTTP-Method-Override"];

            if (!string.IsNullOrEmpty(headerValue))
            {
                context.Request.Method = headerValue;
            }
            else if (!string.IsNullOrEmpty(queryValue))
            {
                context.Request.Method = queryValue;
            }

            await _next.Invoke(context);
        }
    }
}