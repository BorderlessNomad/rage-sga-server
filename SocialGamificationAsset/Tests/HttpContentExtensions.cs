using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Tests
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            // Only accept JSON from the server
            return await content.ReadAsAsync<T>(GetJsonFormatters());
        }

        private static IEnumerable<MediaTypeFormatter> GetJsonFormatters()
        {
            yield return new JsonMediaTypeFormatter();
        }

    }
}