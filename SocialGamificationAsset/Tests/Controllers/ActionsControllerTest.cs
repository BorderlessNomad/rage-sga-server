using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Models;

using Xunit;
using System.Collections.Generic;

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

        [Fact]
        public async Task GetActionsWithNonExistingSession()
        {
            var sessionId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId.ToString());

                // Get actions
                var actionResponse = await client.GetAsync($"/api/actions");
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var fetched = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Session {sessionId} is Invalid.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetAllActions()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get actions
                var actionResponse = await client.GetAsync($"/api/actions");
                Assert.Equal(HttpStatusCode.OK, actionResponse.StatusCode);

                var actions = await actionResponse.Content.ReadAsJsonAsync<IList<Models.Action>>();
                Assert.IsType(typeof(List<Models.Action>), actions);
            }
        }

        [Fact]
        public async Task GetValidAction()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var role = await GetRole();

                var actionForm = new Models.Action
                {
                    Verb = "testerVerb",
                    ActivityId = role.ActivityId,
                    GoalId = role.GoalId
                };

                var actionResponse = await client.PostAsJsonAsync("/api/actions", actionForm);
                Assert.Equal(HttpStatusCode.Created, actionResponse.StatusCode);

                Models.Action action = await actionResponse.Content.ReadAsJsonAsync<Models.Action>();
                Assert.IsType(typeof(Models.Action), action);



                // Get actions
                var actionResponse2 = await client.GetAsync($"/api/actions/{action.Id}");
                Assert.Equal(HttpStatusCode.OK, actionResponse2.StatusCode);

                var actions = await actionResponse2.Content.ReadAsJsonAsync<Models.Action>();
                Assert.IsType(typeof(Models.Action), actions);
            }
        }

        [Fact]
        public async Task UpdateInvalidAction()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var role = await GetRole();

                var actionForm = new Models.Action
                {
                    Verb = "testerVerb",
                    ActivityId = role.ActivityId,
                    GoalId = role.GoalId
                };

                // Get action with Invalid Id
                var actionResponse = await client.PutAsJsonAsync($"/api/actions/{invalidId}", actionForm);
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var content = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No such Action found.", content.Error);
            }
        }

        //test with invalid session
        //test with empty form

        [Fact]
        public async Task UpdateValidAction()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var role = await GetRole();

                var actionForm = new Models.Action
                {
                    Verb = "testerVerb",
                    ActivityId = role.ActivityId,
                    GoalId = role.GoalId
                };

                var actionResponse = await client.PostAsJsonAsync("/api/actions", actionForm);
                Assert.Equal(HttpStatusCode.Created, actionResponse.StatusCode);

                Models.Action action = await actionResponse.Content.ReadAsJsonAsync<Models.Action>();
                Assert.IsType(typeof(Models.Action), action);

                var actionForm2 = new Models.Action
                {
                    Verb = "testerVerb2"
                };

                // Get actions
                var actionResponse2 = await client.PutAsJsonAsync($"/api/actions/{action.Id}", actionForm2);
                Assert.Equal(HttpStatusCode.OK, actionResponse2.StatusCode);

                var actions = await actionResponse2.Content.ReadAsJsonAsync<Models.Action>();
                Assert.IsType(typeof(Models.Action), actions);
            }
        }

        [Fact]
        public async Task CreateActionWithInvalidActivityId()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var role = await GetRole();

                var actionForm = new Models.Action
                {
                    Verb = "testerVerb",
                    ActivityId = new Guid(),
                    GoalId = role.GoalId
                };


                var actionResponse = await client.PostAsJsonAsync("/api/actions", actionForm);
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var content = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Invalid ActivityId.", content.Error);
            }

        }

        [Fact]
        public async Task CreateActionWithInvalidGoalId()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var role = await GetRole();

                var actionForm = new Models.Action
                {
                    Verb = "testerVerb",
                    ActivityId = role.ActivityId,
                    GoalId = new Guid()
                };


                var actionResponse = await client.PostAsJsonAsync("/api/actions", actionForm);
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var content = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Invalid GoalId.", content.Error);
            }

        }

        // invalid session
        // empty form

        [Fact]
        public async Task CreateValidAction()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var role = await GetRole();

                var actionForm = new Models.Action
                {
                    Verb = "testerVerb",
                    ActivityId = role.ActivityId,
                    GoalId = role.GoalId
                };


                var actionResponse = await client.PostAsJsonAsync("/api/actions", actionForm);
                Assert.Equal(HttpStatusCode.Created, actionResponse.StatusCode);

                Models.Action action = await actionResponse.Content.ReadAsJsonAsync<Models.Action>();
                Assert.IsType(typeof(Models.Action), action);
            }

        }

        [Fact]
        public async Task UpdateRewardByAction()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var role = await GetRole();

                var actionForm = new Models.Action
                {
                    Verb = "testVerb",
                };


                var actionResponse = await client.PostAsJsonAsync("/api/actions/send", actionForm);
                Assert.Equal(HttpStatusCode.OK, actionResponse.StatusCode);

                Models.Action action = await actionResponse.Content.ReadAsJsonAsync<Models.Action>();
                Assert.IsType(typeof(Models.Action), action);
            }

        }
    }
}