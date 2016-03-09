using System;
using System.Threading.Tasks;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace SocialGamificationAsset.Middlewares
{
    internal class XHttpHeaderOverrideMiddleware
    {
        private readonly RequestDelegate _next;

        public XHttpHeaderOverrideMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <exception cref="Exception">
        ///     A <see langword="delegate" /> callback throws an exception.
        /// </exception>
        public Task Invoke(HttpContext httpContext)
        {
            var headerValue = httpContext.Request.Headers["X-HTTP-Method-Override"];
            var queryValue = httpContext.Request.Query["X-HTTP-Method-Override"];

            if (!string.IsNullOrEmpty(headerValue))
            {
                httpContext.Request.Method = headerValue;
            }
            else if (!string.IsNullOrEmpty(queryValue))
            {
                httpContext.Request.Method = queryValue;
            }

            return _next.Invoke(httpContext);
        }
    }
}