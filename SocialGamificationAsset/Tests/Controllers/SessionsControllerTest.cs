using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Middlewares;
using SocialGamificationAsset.Models;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class SessionsControllerTest : ControllerTest
    {
        [Fact]
        public async Task LoginWithEmptyForm()
        {
            using (var client = _server.AcceptJson())
            {
                // login with Empty UserForm
                var loginFormEmpty = new UserForm();

                var loginResponse = await client.PostAsJsonAsync("/api/sessions", loginFormEmpty);
                Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);

                var created = await loginResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Either Username or Email is required.", created.Error);
            }
        }

        [Fact]
        public async Task LoginWithNoUser()
        {
            using (var client = _server.AcceptJson())
            {
                // login with No Username/Email
                var loginFormNoUsernameOrEmail = new UserForm { Password = "mayur" };

                var loginResponse = await client.PostAsJsonAsync("/api/sessions", loginFormNoUsernameOrEmail);
                Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);

                var created = await loginResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Either Username or Email is required.", created.Error);
            }
        }

        [Fact]
        public async Task LoginWithNoPassword()
        {
            using (var client = _server.AcceptJson())
            {
                // login with No Password
                var loginWithNoPassword = new UserForm { Username = "mayur" };

                var loginResponse = await client.PostAsJsonAsync("/api/sessions", loginWithNoPassword);
                Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);

                var created = await loginResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Password is required.", created.Error);
            }
        }

        [Fact]
        public async Task LoginWithUnknonUser()
        {
            using (var client = _server.AcceptJson())
            {
                // login with Unknown User
                var loginFormUnkownUser = new UserForm { Username = "unknown", Password = "unknown" };

                var loginResponse = await client.PostAsJsonAsync("/api/sessions", loginFormUnkownUser);
                Assert.Equal(HttpStatusCode.NotFound, loginResponse.StatusCode);

                var created = await loginResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("No such Player found.", created.Error);
            }
        }

        [Fact]
        public async Task LoginWithInvalidPassword()
        {
            using (var client = _server.AcceptJson())
            {
                // login with Invalid Password
                var loginFormInvalidPassword = new UserForm { Username = "mayur", Password = "unknown" };

                var loginResponse = await client.PostAsJsonAsync("/api/sessions", loginFormInvalidPassword);
                Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);

                var created = await loginResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Invalid Login Details.", created.Error);
            }
        }

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
                Assert.Equal(HttpStatusCode.NotFound, sessionResponse.StatusCode);
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
                Assert.Equal(HttpStatusCode.NotFound, sessionResponse.StatusCode);

                var fetched = await sessionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("No such Session found.", fetched.Error);
            }
        }
    }
}