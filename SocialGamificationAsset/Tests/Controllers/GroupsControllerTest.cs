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
        public async Task GetMyGroups_WithInvalidSession()
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
        public async Task GetMyGroups_WithNonExistingSession()
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
        public async Task GetMyGroups_WithValidSession()
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
        public async Task GetActorGroups_WithInvalidActor()
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
        public async Task GetActorGroups_WithValidActor()
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
        public async Task CreateGroup_WithoutName()
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
        public async Task CreateGroup_WithExistingName()
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
        public async Task CreateGroup_WithNoPlayer()
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
        public async Task CreateGroup_WithInvalidPlayers()
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
        public async Task CreateGroup_WithValidPlayers()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);
                Assert.Equal(2, group.Players.Count);
            }
        }

        [Fact]
        public async Task GetGroup_WithInvalidId()
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
        public async Task GetGroup_WithValidId()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                // Get Group with valid id
                groupResponse = await client.GetAsync($"/api/groups/{group.Id}");
                Assert.Equal(HttpStatusCode.OK, groupResponse.StatusCode);

                var response = await groupResponse.Content.ReadAsJsonAsync<Group>();
                Assert.IsType(typeof(Group), response);
                Assert.Equal(group.Id, response.Id);
                Assert.Equal(2, response.Players.Count);
            }
        }

        [Fact]
        public async Task UpdateGroup_WithInvalidId()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var form = new GroupFrom();

                // Update Invalid Group
                var invalidId = Guid.NewGuid();
                var groupResponse = await client.PutAsJsonAsync($"/api/groups/{invalidId}", form);
                Assert.Equal(HttpStatusCode.NotFound, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("No Group found.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateGroup_WithInvalidName()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom { Name = "" };

                // Update Group with empty Name
                groupResponse = await client.PutAsJsonAsync($"/api/groups/{group.Id}", updateForm);
                Assert.Equal(HttpStatusCode.BadRequest, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Group name is required.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateGroup_WithExistingName()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom { Name = "rage" };

                // Update Group with existing Name
                groupResponse = await client.PutAsJsonAsync($"/api/groups/{group.Id}", updateForm);
                Assert.Equal(HttpStatusCode.BadRequest, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Group with this name already exists.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateGroup()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom { Name = $"Test.{currentSeed}", Type = GroupVisibility.InviteOnly };

                // Update Group with valid data
                groupResponse = await client.PutAsJsonAsync($"/api/groups/{group.Id}", updateForm);
                Assert.Equal(HttpStatusCode.OK, groupResponse.StatusCode);

                var response = await groupResponse.Content.ReadAsJsonAsync<Group>();
                Assert.IsType(typeof(Group), response);
                Assert.Equal(group.Id, response.Id);
                Assert.Equal($"Test.{currentSeed}", response.Username);
                Assert.Equal(GroupVisibility.InviteOnly, response.Type);
            }
        }

        [Fact]
        public async Task UpdateGroupPlayer_WithInvalidGroup()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var form = new GroupFrom();

                // Update Group Players with Invalid Group Id
                var invalidId = Guid.NewGuid();
                var groupResponse = await client.PutAsJsonAsync($"/api/groups/{invalidId}/players/{invalidId}", form);
                Assert.Equal(HttpStatusCode.NotFound, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("No Group found.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateGroupPlayer_WithInvalidPlayer()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom();

                // Update Group Players with invalid Player
                var invalidId = Guid.NewGuid();
                groupResponse = await client.PutAsJsonAsync($"/api/groups/{group.Id}/players/{invalidId}", updateForm);
                Assert.Equal(HttpStatusCode.NotFound, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("No such Player found.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateGroupPlayer_WithInvalidAction()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom();

                // Update Group Players with invalid Action
                groupResponse =
                    await client.PutAsJsonAsync($"/api/groups/{group.Id}/players/{mayur.Player.Id}/test", updateForm);
                Assert.Equal(HttpStatusCode.BadRequest, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("'test' is not a valid Action.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateGroupPlayer_AddExistingPlayer()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom();

                // Update Group Players with Add Existing Player
                groupResponse =
                    await client.PutAsJsonAsync($"/api/groups/{group.Id}/players/{matt.Player.Id}/add", updateForm);
                Assert.Equal(HttpStatusCode.BadRequest, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Player with Id {matt.Player.Id} already exists in the Group.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateGroupPlayer_AddNewPlayer()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom();

                // Update Group Players with Add New Player
                var jack = await Login("jack", "jack");
                groupResponse =
                    await client.PutAsJsonAsync($"/api/groups/{group.Id}/players/{jack.Player.Id}/add", updateForm);
                Assert.Equal(HttpStatusCode.OK, groupResponse.StatusCode);

                var response = await groupResponse.Content.ReadAsJsonAsync<Group>();
                Assert.IsType(typeof(Group), response);
                Assert.Equal(group.Id, response.Id);
                Assert.Equal($"Test.{currentSeed}", response.Username);
                Assert.Equal(3, response.Players.Count);
            }
        }

        [Fact]
        public async Task UpdateGroupPlayer_RemoveNonExistingPlayer()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom();

                // Update Group Players with Remove Non-existing Player
                var jack = await Login("jack", "jack");
                groupResponse =
                    await client.PutAsJsonAsync($"/api/groups/{group.Id}/players/{jack.Player.Id}/remove", updateForm);
                Assert.Equal(HttpStatusCode.BadRequest, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Player with Id {jack.Player.Id} exists in the Group.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateGroupPlayer_RemoveExistingPlayer()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom();

                // Update Group Players with Remove Existing Player
                groupResponse =
                    await client.PutAsJsonAsync($"/api/groups/{group.Id}/players/{matt.Player.Id}/remove", updateForm);
                Assert.Equal(HttpStatusCode.OK, groupResponse.StatusCode);

                var response = await groupResponse.Content.ReadAsJsonAsync<Group>();
                Assert.IsType(typeof(Group), response);
                Assert.Equal(group.Id, response.Id);
                Assert.Equal($"Test.{currentSeed}", response.Username);
                Assert.Equal(1, response.Players.Count);
            }
        }

        [Fact]
        public async Task UpdateGroupPlayer_InvalidPlayerAdmin()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom();

                // Update Group Players with Invalid Player Admin
                var jack = await Login("jack", "jack");
                groupResponse =
                    await client.PutAsJsonAsync($"/api/groups/{group.Id}/players/{jack.Player.Id}/admin", updateForm);
                Assert.Equal(HttpStatusCode.BadRequest, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Player with Id {jack.Player.Id} exists in the Group.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateGroupPlayer_ExistingAdmin()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom();

                // Update Group Players with Same Admin
                groupResponse =
                    await client.PutAsJsonAsync($"/api/groups/{group.Id}/players/{mayur.Player.Id}/admin", updateForm);
                Assert.Equal(HttpStatusCode.BadRequest, groupResponse.StatusCode);

                var content = await groupResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"You are already Admin of this Group.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateGroupPlayer_ValidPlayerAdmin()
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
                Assert.Equal(mayur.Player.Id, group.AdminId);

                var updateForm = new GroupFrom();

                // Update Group Players with New Player Admin
                groupResponse =
                    await client.PutAsJsonAsync($"/api/groups/{group.Id}/players/{matt.Player.Id}/admin", updateForm);
                Assert.Equal(HttpStatusCode.OK, groupResponse.StatusCode);

                var response = await groupResponse.Content.ReadAsJsonAsync<Group>();
                Assert.IsType(typeof(Group), response);
                Assert.Equal(group.Id, response.Id);
                Assert.Equal($"Test.{currentSeed}", response.Username);
                Assert.Equal(matt.Player.Id, response.AdminId);
            }
        }
    }
}