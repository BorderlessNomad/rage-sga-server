using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Models;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class ActionsControllerTest : ControllerTest
    {

        [Fact]
        public async Task GetInvalidActionWithNonExistingSession()
        {
            var sessionId = Guid.NewGuid();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId.ToString());

                // Get action with Invalid Id
                var actionResponse = await client.GetAsync($"/api/actions/{invalidId}");
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var fetched = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Session {sessionId} is Invalid.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetInvalidAction()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get action with Invalid Id
                var actionResponse = await client.GetAsync($"/api/actions/{invalidId}");
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var content = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Action found.", content.Error);
            }
        }
    }
}