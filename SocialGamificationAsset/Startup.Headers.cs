﻿using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.Threading.Tasks;

namespace SocialGamificationAsset
{
	public partial class Startup
	{
		private static void ConfigureHeadersOverride(IApplicationBuilder application)
		{
			application.UseMiddleware<XHttpHeaderOverride>();
		}
	}

	public class XHttpHeaderOverride
	{
		private readonly RequestDelegate _next;

		public XHttpHeaderOverride(RequestDelegate next)
		{
			_next = next;
		}

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