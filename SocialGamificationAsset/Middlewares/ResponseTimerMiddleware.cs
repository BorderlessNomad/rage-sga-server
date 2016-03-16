using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace SocialGamificationAsset.Middlewares
{
    internal class ResponseTimerMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// </summary>
        /// <param name="next"></param>
        public ResponseTimerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var timer = Stopwatch.StartNew();
            context.Response.OnStarting(
                state =>
                    {
                        timer.Stop();
                        context.Response.Headers.Add(
                            "X-Response-Time",
                            new[] { timer.ElapsedMilliseconds.ToString() + "ms" });

                        return Task.FromResult<object>(null);
                    },
                context.Response);

            await _next.Invoke(context);
        }
    }
}