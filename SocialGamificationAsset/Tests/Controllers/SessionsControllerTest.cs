using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Models;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class SessionsControllerTest : ControllerTest
    {
        [Fact]
        public async Task LoginThenObtainSession()
        {
            using (var client = _server.AcceptJson())
            {
                // login
                var loginForm = new UserForm { Username = "mayur", Password = "mayur" };

                var loginResponse = await client.PostAsJsonAsync("/api/sessions", loginForm);
                Assert.True(loginResponse.IsSuccessStatusCode);

                var created = await loginResponse.Content.ReadAsJsonAsync<Session>();
                Assert.NotEqual(Guid.Empty, created.Id);
                Assert.Equal(loginForm.Username, created.Player.Username);

                // get session
                var sessionResponse = await client.GetAsync($"/api/sessions/{created.Id}");
                Assert.True(sessionResponse.IsSuccessStatusCode);

                var fetched = await sessionResponse.Content.ReadAsJsonAsync<Session>();
                Assert.Equal(created.Id, fetched.Id);
                Assert.Equal(loginForm.Username, fetched.Player.Username);
            }
        }

        [Fact]
        public async Task LoginThenLogout()
        {
            using (var client = _server.AcceptJson())
            {
                // login 
                var loginForm = new UserForm { Username = "mayur", Password = "mayur" };

                var loginResponse = await client.PostAsJsonAsync("/api/sessions", loginForm);
                Assert.True(loginResponse.IsSuccessStatusCode);

                var created = await loginResponse.Content.ReadAsJsonAsync<Session>();
                Assert.NotEqual(Guid.Empty, created.Id);
                Assert.Equal(loginForm.Username, created.Player.Username);

                // logout
                var logoutResponse = await client.DeleteAsync($"/api/sessions/{created.Id}");
                Assert.True(logoutResponse.IsSuccessStatusCode);

                var deleted = await logoutResponse.Content.ReadAsJsonAsync<Session>();
                Assert.Equal(created.Id, deleted.Id);
                Assert.True(deleted.IsExpired);
            }
        }

        [Fact]
        public async Task ObtainSessionWithMalformedString()
        {
            using (var client = _server.AcceptJson())
            {
                var sessionId = "test";

                // get session
                var sessionResponse = await client.GetAsync($"/api/sessions/{sessionId}");
                Assert.Equal(sessionResponse.StatusCode, HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public async Task ObtainSessionWithInvalidGuid()
        {
            using (var client = _server.AcceptJson())
            {
                var sessionId = Guid.NewGuid();

                // get session
                var sessionResponse = await client.GetAsync($"/api/sessions/{sessionId}");
                Assert.Equal(sessionResponse.StatusCode, HttpStatusCode.NotFound);

                var fetched = await sessionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("No such Session found.", fetched.Error);
            }
        }
    }
}