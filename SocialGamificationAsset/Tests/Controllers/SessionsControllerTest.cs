using System;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Models;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class SessionsControllerTest : ControllerTest
    {
        [Fact]
        public async Task login_then_get_session()
        {
            using (var client = _server.AcceptJson())
            {
                var loginForm = new UserForm { Username = "mayur", Password = "mayur" };

                var loginResponse = await client.PostAsJsonAsync("/sessions", loginForm);
                Assert.True(loginResponse.IsSuccessStatusCode);

                var created = await loginResponse.Content.ReadAsJsonAsync<Session>();
                Assert.NotEqual(Guid.Empty, created.Id);
                Assert.Equal(loginForm.Username, created.Player.Username);

                var sessionResponse = await client.GetAsync($"/sessions/{created.Id}");
                Assert.True(sessionResponse.IsSuccessStatusCode);

                var fetched = await sessionResponse.Content.ReadAsJsonAsync<Session>();
                Assert.Equal(created.Id, fetched.Id);
                Assert.Equal(loginForm.Username, fetched.Player.Username);
            }
        }
    }
}