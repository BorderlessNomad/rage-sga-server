using System.Net.Http;
using System.Net.Http.Headers;

namespace SocialGamificationAsset.Policies
{
    public static class HttpClientExtensions
    {
        public static HttpClient AcceptJson(HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}