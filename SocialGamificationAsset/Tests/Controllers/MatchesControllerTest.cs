using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class MatchesControllerTest : ControllerTest
    {
        [Fact]
        public async Task GetMyMatchesWithoutSession()
        {
            using (var client = _server.AcceptJson())
            {
                // get my matches without session header
                var matchesResponse = await client.GetAsync($"/api/matches");
                Assert.Equal(HttpStatusCode.Unauthorized, matchesResponse.StatusCode);

                var fetched = await matchesResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No {SessionAuthorizeFilter.SessionHeaderName} Header found.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetMyMatchesWithSession()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                // get my matches with session header
                var matchesResponse = await client.GetAsync("/api/matches");
                Assert.Equal(HttpStatusCode.OK, matchesResponse.StatusCode);

                var matches = await matchesResponse.Content.ReadAsJsonAsync<IList<Match>>();
                Assert.IsType(typeof(List<Match>), matches);
            }
        }
    }
}