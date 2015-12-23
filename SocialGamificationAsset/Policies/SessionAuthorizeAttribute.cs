using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Policies
{
	public interface ISessionAuthorizeFilter { }

	public class SessionAuthorizeFilter : IAsyncAuthorizationFilter, ISessionAuthorizeFilter
	{
		public const string SessionHeaderName = "X-HTTP-SESSION";

		public virtual async Task OnAuthorizationAsync(AuthorizationContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			// Allow Anonymous skips all authorization
			if (context.Filters.Any(item => item is IAllowAnonymousFilter))
			{
				return;
			}

			HttpRequest request = context.HttpContext.Request;

			StringValues header;
			bool headerExists = request.Headers.TryGetValue(SessionHeaderName, out header);
			if (!headerExists)
			{
				context.Result = new HttpNotFoundResult();
				return;
			}

			Guid token;
			bool isValidGuid = Guid.TryParse(header.FirstOrDefault(), out token);
			if (!isValidGuid) // Header value must be valid Guid
			{
				context.Result = new HttpNotFoundResult();
				return;
			}

			Debug.WriteLine(token);
		}
	}
}
