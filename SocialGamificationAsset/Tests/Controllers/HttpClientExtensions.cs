using System.Net.Http;
using System.Net.Http.Headers;

using SocialGamificationAsset.Policies;

namespace SocialGamificationAsset.Tests.Controllers
{
    public static class HttpClientExtensions
    {
        public static HttpClient AcceptJson(this HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public static HttpClient AddSessionHeader(this HttpClient client, string sessionId)
        {
            client.DefaultRequestHeaders.Add(SessionAuthorizeFilter.SessionHeaderName, sessionId);

            return client;
        }
    }
}