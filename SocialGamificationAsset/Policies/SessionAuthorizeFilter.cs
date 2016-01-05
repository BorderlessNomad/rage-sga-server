using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using SocialGamificationAsset.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Policies
{
	public interface ISessionAuthorizeFilter { }

	public class SessionAuthorizeFilter : IAsyncAuthorizationFilter, ISessionAuthorizeFilter
	{
		public const string SessionHeaderName = "X-HTTP-SESSION";
		public const string DocumentationApiKey = "api_key";
		public const string DocumentationApiValue = "00000000-0000-0000-0000-000000000000";

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

			HttpContext httpContext = context.HttpContext;

			// Check if Request is from Documentation Generator
			String documentationApiKey = httpContext.Request.Query[DocumentationApiKey];
			if (!String.IsNullOrEmpty(documentationApiKey))
			{
				if (documentationApiKey.Equals(DocumentationApiValue, StringComparison.InvariantCultureIgnoreCase))
				{
					return;
				}
				else
				{
					context.Result = new BadRequestObjectResult("Invalid value for " + DocumentationApiKey);
					return;
				}
			}

			StringValues header;
			bool headerExists = httpContext.Request.Headers.TryGetValue(SessionHeaderName, out header);

			// Check if Session Header exists
			if (!headerExists)
			{
				context.Result = new HttpNotFoundObjectResult("No " + SessionHeaderName + " found.");
				return;
			}

			Guid token;
			bool isValidGuid = Guid.TryParse(header.FirstOrDefault(), out token);

			// Token value must be valid Guid
			if (!isValidGuid)
			{
				context.Result = new HttpNotFoundObjectResult("Invalid " + SessionHeaderName + ".");
				return;
			}

			Session localSession = httpContext.Session.GetObjectFromJson<Session>("__session");

			// If 'active' session already exists skip DB call
			if (localSession != null && localSession.Id.Equals(token))
			{
				return;
			}

			SocialGamificationAssetContext db = httpContext.RequestServices.GetRequiredService<SocialGamificationAssetContext>();
			if (db == null)
			{
				throw new ApplicationException("Unable to find requested database service.");
			}

			Session session = await db.Sessions.FindAsync(token);

			// Find Session
			if (session == null)
			{
				context.Result = new HttpNotFoundObjectResult("Session " + token + " is Invalid.");
				return;
			}

			// Set right Session
			httpContext.Session.SetObjectAsJson("__session", session);
		}
	}

	public static class SessionExtensions
	{
		public static void SetObjectAsJson(this ISession session, string key, object value)
		{
			session.SetString(key, JsonConvert.SerializeObject(value));
		}

		public static T GetObjectFromJson<T>(this ISession session, string key)
		{
			var value = session.GetString(key);

			return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
		}
	}
}
