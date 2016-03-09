using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace SocialGamificationAsset.Middlewares
{
    internal class ResponseTimerMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseTimerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            context.Response.OnStarting(
                state =>
                    {
                        sw.Stop();
                        context.Response.Headers.Add(
                            "X-Response-Time",
                            new[] { sw.ElapsedMilliseconds.ToString() + "ms" });

                        return Task.FromResult<object>(null);
                    },
                context.Response);

            await _next.Invoke(context);
        }
    }
}