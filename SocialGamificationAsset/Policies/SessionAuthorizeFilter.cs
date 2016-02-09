using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Policies
{
    public interface ISessionAuthorizeFilter
    {
    }

    public class SessionAuthorizeFilter : IAsyncAuthorizationFilter, ISessionAuthorizeFilter
    {
        public const string SessionHeaderName = "X-HTTP-Session";

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

            var httpContext = context.HttpContext;

            // Check if Request is from Documentation Generator
            string documentationApiKey = httpContext.Request.Query[DocumentationApiKey];
            if (!string.IsNullOrEmpty(documentationApiKey))
            {
                if (documentationApiKey.Equals(DocumentationApiValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }
                context.Result = new BadRequestObjectResult("Invalid value for " + DocumentationApiKey);
                return;
            }

            StringValues header;
            var headerExists = httpContext.Request.Headers.TryGetValue(SessionHeaderName, out header);

            // Check if Session Header exists
            if (!headerExists)
            {
                context.Result = new ContentResult
                                 {
                                     StatusCode = StatusCodes.Status401Unauthorized,
                                     Content = "No " + SessionHeaderName + " found."
                                 };
                return;
            }

            Guid token;
            var isValidGuid = Guid.TryParse(header.FirstOrDefault(), out token);

            // Token value must be valid Guid
            if (!isValidGuid)
            {
                context.Result = new ContentResult
                                 {
                                     StatusCode = StatusCodes.Status401Unauthorized,
                                     Content = "Invalid " + SessionHeaderName + "."
                                 };
                return;
            }

            Session localSession = httpContext.Session.GetObjectFromJson<Session>("__session");

            // If 'active' session already exists skip DB call
            if (localSession != null && localSession.Id.Equals(token) && localSession.Player != null)
            {
                return;
            }

            var db = httpContext.RequestServices.GetRequiredService<SocialGamificationAssetContext>();
            if (db == null)
            {
                context.Result = new ContentResult
                                 {
                                     StatusCode = StatusCodes.Status503ServiceUnavailable,
                                     Content = "Unable to find requested database service."
                                 };
                return;
            }

            var session = await db.Sessions.Where(s => s.Id.Equals(token)).Include(s => s.Player).FirstOrDefaultAsync();

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