using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Models;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class SessionsControllerTest : ControllerTest
    {
        [Fact]
        public async Task Login()
        {
            using (var client = _server.AcceptJson())
            {
                var response = await client.GetAsync("/sessions");
                var result = await response.Content.ReadAsStringAsync();
                //var result = await response.Content.ReadAsJsonAsync<Session>();

                Assert.NotNull(result);
            }
        }
    }
}