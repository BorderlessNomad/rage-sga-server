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
    public class GroupsControllerTest : ControllerTest
    {
        [Fact]
        public async Task GetMyGroupsWithInvalidSession()
        {
            var sessionId = "unknown";

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId);

                // Get My Groups with invalid session header
                var groupResponse = await client.GetAsync("/api/groups");
                Assert.Equal(HttpStatusCode.Unauthorized, groupResponse.StatusCode);

                var fetched = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Invalid {SessionAuthorizeFilter.SessionHeaderName} Header.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetMyGroupsWithNonExistingSession()
        {
            var sessionId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId);

                // Get My Groups with non-existing session header
                var groupResponse = await client.GetAsync("/api/groups");
                Assert.Equal(HttpStatusCode.NotFound, groupResponse.StatusCode);

                var fetched = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Session {sessionId} is Invalid.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetMyGroupsWithValidSession()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                // Get My Groups with valid session header
                var groupResponse = await client.GetAsync("/api/groups");
                Assert.Equal(HttpStatusCode.OK, groupResponse.StatusCode);

                var groups = await groupResponse.Content.ReadAsJsonAsync<IList<Group>>();
                Assert.IsType(typeof(List<Group>), groups);
            }
        }

        [Fact]
        public async Task GetActorGroupsWithInvalidActor()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                // Get Actor's Groups with invalid Actor
                var invalidId = Guid.NewGuid();
                var groupResponse = await client.GetAsync($"/api/groups/actor/{invalidId}");
                Assert.Equal(HttpStatusCode.NotFound, groupResponse.StatusCode);

                var fetched = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No such Player found.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetActorGroupsWithValidActor()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                // Get Actor's Groups valid Actor
                var groupResponse = await client.GetAsync($"/api/groups/actor/{session.Player.Id}");
                Assert.Equal(HttpStatusCode.OK, groupResponse.StatusCode);

                var groups = await groupResponse.Content.ReadAsJsonAsync<IList<Group>>();
                Assert.IsType(typeof(List<Group>), groups);
            }
        }

        [Fact]
        public async Task CreateGroupWithoutName()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var form = new GroupFrom();

                // Create Group without Name
                var groupResponse = await client.PostAsJsonAsync($"/api/groups", form);
                Assert.Equal(HttpStatusCode.BadRequest, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Group name is required.", content.Error);
            }
        }

        [Fact]
        public async Task CreateGroupWithExistingName()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var form = new GroupFrom { Name = "rage" };

                // Create Group with existing Name
                var groupResponse = await client.PostAsJsonAsync($"/api/groups", form);
                Assert.Equal(HttpStatusCode.BadRequest, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Group with this name already exists.", content.Error);
            }
        }

        [Fact]
        public async Task CreateGroupWithNoPlayer()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var currentSeed = Guid.NewGuid();
                var form = new GroupFrom { Name = $"Test.{currentSeed}", Players = new List<Guid>() };

                // Create Group with No Player
                var groupResponse = await client.PostAsJsonAsync($"/api/groups", form);
                Assert.Equal(HttpStatusCode.BadRequest, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Group requires minimum 1 Player.", content.Error);
            }
        }

        [Fact]
        public async Task CreateGroupWithInvalidPlayers()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var currentSeed = Guid.NewGuid();
                var invalidPlayer = Guid.NewGuid();
                var form = new GroupFrom
                {
                    Name = $"Test.{currentSeed}",
                    Players = new List<Guid> { session.Player.Id, invalidPlayer }
                };

                // Create Group with Invalid Players
                var groupResponse = await client.PostAsJsonAsync($"/api/groups", form);
                Assert.Equal(HttpStatusCode.NotFound, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Player with Id {invalidPlayer} exists.", content.Error);
            }
        }

        [Fact]
        public async Task CreateGroupWithValidPlayers()
        {
            var mayur = await Login();
            var matt = await Login("matt", "matt");

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(mayur.Id);

                var currentSeed = Guid.NewGuid();
                var form = new GroupFrom
                {
                    Name = $"Test.{currentSeed}",
                    Type = GroupVisibility.Invisible,
                    Players = new List<Guid> { mayur.Player.Id, matt.Player.Id }
                };

                // Create Group with valid Players
                var groupResponse = await client.PostAsJsonAsync($"/api/groups", form);
                Assert.Equal(HttpStatusCode.Created, groupResponse.StatusCode);

                var group = await groupResponse.Content.ReadAsJsonAsync<Group>();
                Assert.IsType(typeof(Group), group);
                Assert.Equal(GroupVisibility.Invisible, group.Type);
                Assert.Equal(mayur.Player.Id, group.OwnerId);
            }
        }

        [Fact]
        public async Task GetGroupWithInvalidId()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                // Get Group with invalid id
                var invalidId = Guid.NewGuid();
                var groupResponse = await client.GetAsync($"/api/groups/{invalidId}");
                Assert.Equal(HttpStatusCode.NotFound, groupResponse.StatusCode);

                var fetched = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Group found.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetGroupWithValidId()
        {
            var mayur = await Login();
            var matt = await Login("matt", "matt");

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(mayur.Id);

                var currentSeed = Guid.NewGuid();
                var form = new GroupFrom
                {
                    Name = $"Test.{currentSeed}",
                    Type = GroupVisibility.Invisible,
                    Players = new List<Guid> { mayur.Player.Id, matt.Player.Id }
                };

                // Create Group with valid Players
                var groupResponse = await client.PostAsJsonAsync($"/api/groups", form);
                Assert.Equal(HttpStatusCode.Created, groupResponse.StatusCode);

                var group = await groupResponse.Content.ReadAsJsonAsync<Group>();
                Assert.IsType(typeof(Group), group);
                Assert.Equal(mayur.Player.Id, group.OwnerId);

                // Get Group with valid id
                groupResponse = await client.GetAsync($"/api/groups/{group.Id}");
                Assert.Equal(HttpStatusCode.OK, groupResponse.StatusCode);

                var response = await groupResponse.Content.ReadAsJsonAsync<Group>();
                Assert.IsType(typeof(Group), response);
                Assert.Equal(group.Id, response.Id);
            }
        }
    }
}