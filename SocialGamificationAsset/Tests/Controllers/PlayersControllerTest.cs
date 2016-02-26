using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Models;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class PlayersControllerTest : ControllerTest
    {
        [Fact]
        public async Task WhoAmIWithValidSession()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var playerResponse = await client.GetAsync("/api/players");
                Assert.Equal(HttpStatusCode.OK, playerResponse.StatusCode);

                var player = await playerResponse.Content.ReadAsJsonAsync<Player>();
                Assert.Equal(session.Player.Id, player.Id);
            }
        }
    }
}
