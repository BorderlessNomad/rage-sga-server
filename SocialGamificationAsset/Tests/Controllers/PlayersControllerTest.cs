using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;

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
                client.AcceptJson().AddSessionHeader(session.Id);

                var playerResponse = await client.GetAsync("/api/players");
                Assert.Equal(HttpStatusCode.OK, playerResponse.StatusCode);

                var player = await playerResponse.Content.ReadAsJsonAsync<Player>();
                Assert.Equal(session.Player.Id, player.Id);
            }
        }

        [Fact]
        public async Task GetPlayerWithInvalidSession()
        {
            var sessionId = "unknown";

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId);

                // Get Player with invalid session header
                var playerResponse = await client.GetAsync("/api/players");
                Assert.Equal(HttpStatusCode.Unauthorized, playerResponse.StatusCode);

                var fetched = await playerResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Invalid {SessionAuthorizeFilter.SessionHeaderName} Header.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetPlayerWithNonExistingSession()
        {
            var sessionId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId);

                // Get Player with non-existing session header
                var playerResponse = await client.GetAsync("/api/players");
                Assert.Equal(HttpStatusCode.NotFound, playerResponse.StatusCode);

                var fetched = await playerResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Session {sessionId} is Invalid.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetPlayerWithNonExistingId()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                // Get Player with non-existing session Id
                var playerResponse = await client.GetAsync($"/api/players/{invalidId}");
                Assert.Equal(HttpStatusCode.NotFound, playerResponse.StatusCode);

                var fetched = await playerResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No such Player found.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetPlayerWithValidId()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                // Get Player with valid & existing session Id
                var playerResponse = await client.GetAsync($"/api/players/{session.Player.Id}");
                Assert.Equal(HttpStatusCode.OK, playerResponse.StatusCode);

                var player = await playerResponse.Content.ReadAsJsonAsync<Player>();
                Assert.Equal(session.Player.Id, player.Id);
            }
        }

        [Fact]
        public async Task AddPlayerWithoutUsernameOrEmail()
        {
            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                var form = new UserForm();

                // Add Player without Username or Email
                var playerResponse = await client.PostAsJsonAsync($"/api/players", form);
                Assert.Equal(HttpStatusCode.BadRequest, playerResponse.StatusCode);

                var content = await playerResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Either Username or Email is required.", content.Error);
            }
        }

        [Fact]
        public async Task AddPlayerWithoutPassword()
        {
            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                var form = new UserForm { Username = "AddPlayerWithoutPassword" };

                // Add Player without Passwrod
                var playerResponse = await client.PostAsJsonAsync($"/api/players", form);
                Assert.Equal(HttpStatusCode.BadRequest, playerResponse.StatusCode);

                var content = await playerResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Password is required.", content.Error);
            }
        }

        [Fact]
        public async Task AddPlayerWithExistingUsername()
        {
            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                var form = new UserForm { Username = "mayur", Password = "mayur" };

                // Add Player with existing Username
                var playerResponse = await client.PostAsJsonAsync($"/api/players", form);
                Assert.Equal(HttpStatusCode.BadRequest, playerResponse.StatusCode);

                var content = await playerResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Player with this Username already exists.", content.Error);
            }
        }

        [Fact]
        public async Task AddPlayerWithExistingEmail()
        {
            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                var form = new UserForm { Email = "mayur@playgen.com", Password = "mayur" };

                // Add Player with existing Email
                var playerResponse = await client.PostAsJsonAsync($"/api/players", form);
                Assert.Equal(HttpStatusCode.BadRequest, playerResponse.StatusCode);

                var content = await playerResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Player with this Email already exists.", content.Error);
            }
        }

        [Fact]
        public async Task AddPlayer()
        {
            var currentSeed = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                var form = new UserForm
                {
                    Username = $"Test.{currentSeed}",
                    Email = $"test.{currentSeed}@playgen.com",
                    Password = "test"
                };

                // Add Player with valid data
                var playerResponse = await client.PostAsJsonAsync($"/api/players", form);
                Assert.Equal(HttpStatusCode.Created, playerResponse.StatusCode);

                var player = await playerResponse.Content.ReadAsJsonAsync<Player>();
                Assert.Equal($"Test.{currentSeed}", player.Username);
            }
        }

        [Fact]
        public async Task UpdatePlayerWithoutId()
        {
            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                var form = new UserForm();

                // Update Player without Id
                var playerResponse = await client.PutAsJsonAsync($"/api/players", form);
                Assert.Equal(HttpStatusCode.NotFound, playerResponse.StatusCode);
            }
        }

        [Fact]
        public async Task UpdatePlayerWithInvaliId()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var form = new UserForm();

                // Update Player with invalid Id
                var playerResponse = await client.PutAsJsonAsync($"/api/players/{invalidId}", form);
                Assert.Equal(HttpStatusCode.NotFound, playerResponse.StatusCode);

                var content = await playerResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("No Player found.", content.Error);
            }
        }

        [Fact]
        public async Task UpdatePlayerWithExistingUsername()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var form = new UserForm { Username = "jack" };

                // Update Player with existing Username
                var playerResponse = await client.PutAsJsonAsync($"/api/players/{session.Player.Id}", form);
                Assert.Equal(HttpStatusCode.BadRequest, playerResponse.StatusCode);

                var content = await playerResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Player with this Username already exists.", content.Error);
            }
        }

        [Fact]
        public async Task UpdatePlayerWithExistingEmail()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var form = new UserForm { Email = "jack@playgen.com" };

                // Update Player with existing Email
                var playerResponse = await client.PutAsJsonAsync($"/api/players/{session.Player.Id}", form);
                Assert.Equal(HttpStatusCode.BadRequest, playerResponse.StatusCode);

                var content = await playerResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Player with this Email already exists.", content.Error);
            }
        }

        [Fact]
        public async Task UpdatePlayer()
        {
            var session = await Login();
            var currentSeed = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var form = new UserForm
                {
                    Username = $"Test.{currentSeed}",
                    Email = $"test.{currentSeed}@playgen.com",
                    Password = "test"
                };

                // Add Player with valid data
                var playerResponse = await client.PostAsJsonAsync($"/api/players", form);
                Assert.Equal(HttpStatusCode.Created, playerResponse.StatusCode);

                var player = await playerResponse.Content.ReadAsJsonAsync<Player>();
                Assert.Equal($"Test.{currentSeed}", player.Username);

                // Update Player with new data
                var newSeed = Guid.NewGuid();
                form.Username = $"Test.{newSeed}";
                form.Email = $"test.{newSeed}@playgen.com";

                playerResponse = await client.PutAsJsonAsync($"/api/players/{player.Id}", form);
                Assert.Equal(HttpStatusCode.OK, playerResponse.StatusCode);

                player = await playerResponse.Content.ReadAsJsonAsync<Player>();
                Assert.Equal($"Test.{newSeed}", player.Username);
                Assert.Equal($"test.{newSeed}@playgen.com", player.Email);
            }
        }

        [Fact]
        public async Task DeleteInvalidPlayer()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                // Delete Player with Invalid Id
                var invalidPlayerId = Guid.NewGuid();
                var playerResponse = await client.DeleteAsync($"/api/players/{invalidPlayerId}");
                Assert.Equal(HttpStatusCode.NotFound, playerResponse.StatusCode);

                var content = await playerResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Player found.", content.Error);
            }
        }

        [Fact]
        public async Task DeleteValidPlayer()
        {
            var session = await Login();
            var currentSeed = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var form = new UserForm
                {
                    Username = $"Test.{currentSeed}",
                    Email = $"test.{currentSeed}@playgen.com",
                    Password = "test"
                };

                // Add Player with valid data
                var playerResponse = await client.PostAsJsonAsync($"/api/players", form);
                Assert.Equal(HttpStatusCode.Created, playerResponse.StatusCode);

                var player = await playerResponse.Content.ReadAsJsonAsync<Player>();
                Assert.Equal($"Test.{currentSeed}", player.Username);

                // Delete Player
                playerResponse = await client.DeleteAsync($"/api/players/{player.Id}");
                Assert.Equal(HttpStatusCode.OK, playerResponse.StatusCode);

                player = await playerResponse.Content.ReadAsJsonAsync<Player>();
                Assert.False(player.IsEnabled);
            }
        }
    }
}