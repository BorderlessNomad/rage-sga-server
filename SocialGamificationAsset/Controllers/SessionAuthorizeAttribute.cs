using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Newtonsoft.Json;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SocialGamificationAsset.Controllers
{
	public class SessionAuthorizeAttribute : AuthorizeAttribute
	{
		private readonly SocialGamificationAssetContext _context;

		private readonly IHttpContextAccessor _httpContextAccessor;

		private ISession _session => _httpContextAccessor.HttpContext.Session;

		public SessionAuthorizeAttribute(IHttpContextAccessor httpContextAccessor, SocialGamificationAssetContext context)
		{
			_httpContextAccessor = httpContextAccessor;
			_context = context;
		}

		protected override bool IsAuthorized(HttpActionContext actionContext)
		{
			bool authorized = base.IsAuthorized(actionContext);
			if (authorized) // Check if Session is already authorized
			{
				return true;
			}

			IEnumerable<string> headers;
			bool hasHeader = actionContext.Request.Headers.TryGetValues("X-HTTP-SESSION", out headers);
			if (!hasHeader) // Check if Header contains X-HTTP-SESSION
			{
				return false;
			}

			Guid token;
			bool isValidGuid = Guid.TryParse(headers.FirstOrDefault(), out token);
			if (!isValidGuid) // Header value must be valid Guid
			{
				return false;
			}

			Session session = _context.Sessions.Find(token);
			if (session == null) // Session must exists
			{
				return false;
			}

			var sessionInMemory = _session.GetObjectFromJson<Session>(token.ToString());
			Debug.WriteLine(sessionInMemory);

			return true;
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
