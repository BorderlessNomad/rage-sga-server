using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Models;

using Xunit;

using Action = SocialGamificationAsset.Models.Action;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class ActionsControllerTest : ControllerTest
    {
        protected async Task<Action> CreateTestAction(string verb = "Created for test goal and activity")
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var action = new Action
                {
                    Verb = verb,
                    Goal =
                        new Goal
                        {
                            Concern = new ConcernMatrix { Coordinates = new Matrix { X = 1, Y = 1 }, Category = 0 },
                            RewardResource =
                                new RewardResourceMatrix { Coordinates = new Matrix { X = 2, Y = 2 }, Category = 0 },
                            Feedback = new GoalFeedback { Threshold = 0, Target = 0, Direction = 0 },
                            Description = "Created for test cases"
                        },
                    Activity = new Activity { Name = "Testing" }
                };
                var actionResponse = await client.PostAsJsonAsync("/api/actions", action);
                Assert.Equal(HttpStatusCode.Created, actionResponse.StatusCode);

                var created = await actionResponse.Content.ReadAsJsonAsync<Action>();

                return created;
            }
        }

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
        public async Task GetInvalidActionRelationWithNonExistingSession()
        {
            var sessionId = Guid.NewGuid();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId.ToString());

                // Get action with Invalid Id
                var actionResponse = await client.GetAsync($"/api/actions/{invalidId}/relations");
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var fetched = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Session {sessionId} is Invalid.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetInvalidActionRelation()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get action with Invalid Id
                var actionResponse = await client.GetAsync($"/api/actions/{invalidId}/relations");
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var content = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No ActionRelation found.", content.Error);
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

                var actions = await actionResponse.Content.ReadAsJsonAsync<IList<Action>>();
                Assert.IsType(typeof(List<Action>), actions);
            }
        }

        [Fact]
        public async Task GetValidAction()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newAction = await CreateTestAction();

                var actionForm = new Action
                {
                    Verb = "testerVerb",
                    ActivityId = newAction.ActivityId,
                    GoalId = newAction.GoalId
                };

                var actionResponse = await client.PostAsJsonAsync("/api/actions", actionForm);
                Assert.Equal(HttpStatusCode.Created, actionResponse.StatusCode);

                var action = await actionResponse.Content.ReadAsJsonAsync<Action>();
                Assert.IsType(typeof(Action), action);

                // Get actions
                var actionResponse2 = await client.GetAsync($"/api/actions/{action.Id}");
                Assert.Equal(HttpStatusCode.OK, actionResponse2.StatusCode);

                var actions = await actionResponse2.Content.ReadAsJsonAsync<Action>();
                Assert.IsType(typeof(Action), actions);
            }
        }

        [Fact]
        public async Task GetValidActionRelation()
        {
            var session = await Login();
            var newAction = await CreateTestAction("Verb test" + Guid.NewGuid());

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newAT = new AttributeType { Name = "testAttribute", DefaultValue = 0f, Type = 0 };
                var atResponse = await client.PostAsJsonAsync("/api/attributes/types", newAT);
                Assert.Equal(HttpStatusCode.Created, atResponse.StatusCode);
                var attribute = await atResponse.Content.ReadAsJsonAsync<AttributeType>();
                var newAR = new ActionRelation
                {
                    ActionId = newAction.Id,
                    Relationship = 0,
                    ConcernChange = new Matrix { X = 0, Y = 0 },
                    RewardResourceChange = new Matrix { X = 0, Y = 0 },
                    AttributeChanges =
                        new List<Reward>
                        {
                            new Reward
                            {
                                AttributeTypeId = attribute.Id,
                                TypeReward = RewardType.Modify,
                                Value = 1f,
                                Status = 0,
                                GoalId = newAction.GoalId
                            }
                        }
                };
                var arResponse = await client.PostAsJsonAsync("/api/actions/relations", newAR);
                Assert.Equal(HttpStatusCode.Created, arResponse.StatusCode);

                var action = await arResponse.Content.ReadAsJsonAsync<ActionRelation>();
                Assert.IsType(typeof(ActionRelation), action);

                // Get actions
                var actionResponse2 = await client.GetAsync($"/api/actions/{action.Id}/relations");
                Assert.Equal(HttpStatusCode.OK, actionResponse2.StatusCode);

                var actions = await actionResponse2.Content.ReadAsJsonAsync<ActionRelation>();
                Assert.IsType(typeof(ActionRelation), actions);
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

                var newAction = await CreateTestAction();

                var actionForm = new Action
                {
                    Verb = "testerVerb",
                    ActivityId = newAction.ActivityId,
                    GoalId = newAction.GoalId
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

                var newAction = await CreateTestAction();

                var actionForm = new Action
                {
                    Verb = "testerVerb",
                    ActivityId = newAction.ActivityId,
                    GoalId = newAction.GoalId
                };

                var actionResponse = await client.PostAsJsonAsync("/api/actions", actionForm);
                Assert.Equal(HttpStatusCode.Created, actionResponse.StatusCode);

                var action = await actionResponse.Content.ReadAsJsonAsync<Action>();
                Assert.IsType(typeof(Action), action);

                var actionForm2 = new Action { Verb = "testerVerb2" };

                // Get actions
                var actionResponse2 = await client.PutAsJsonAsync($"/api/actions/{action.Id}", actionForm2);
                Assert.Equal(HttpStatusCode.OK, actionResponse2.StatusCode);

                var actions = await actionResponse2.Content.ReadAsJsonAsync<Action>();
                Assert.IsType(typeof(Action), actions);
            }
        }

        [Fact]
        public async Task CreateActionWithInvalidActivityId()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newAction = await CreateTestAction();

                var actionForm = new Action { Verb = "testerVerb", ActivityId = new Guid(), GoalId = newAction.GoalId };

                var actionResponse = await client.PostAsJsonAsync("/api/actions", actionForm);
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var content = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Activity found", content.Error);
            }
        }

        [Fact]
        public async Task CreateActionWithInvalidGoalId()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newAction = await CreateTestAction();

                var actionForm = new Action
                {
                    Verb = "testerVerb",
                    ActivityId = newAction.ActivityId,
                    GoalId = new Guid()
                };

                var actionResponse = await client.PostAsJsonAsync("/api/actions", actionForm);
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var content = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Goal found", content.Error);
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

                var newAction = await CreateTestAction();

                var actionForm = new Action
                {
                    Verb = "testerVerb",
                    ActivityId = newAction.ActivityId,
                    GoalId = newAction.GoalId
                };

                var actionResponse = await client.PostAsJsonAsync("/api/actions", actionForm);
                Assert.Equal(HttpStatusCode.Created, actionResponse.StatusCode);

                var action = await actionResponse.Content.ReadAsJsonAsync<Action>();
                Assert.IsType(typeof(Action), action);
            }
        }

        [Fact]
        public async Task CreateActionRelationWithInvalidActionId()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newAR = new ActionRelation
                {
                    ActionId = new Guid(),
                    Relationship = 0,
                    ConcernChange = new Matrix { X = 0, Y = 0 },
                    RewardResourceChange = new Matrix { X = 0, Y = 0 }
                };
                var arResponse = await client.PostAsJsonAsync("/api/actions/relations", newAR);
                Assert.Equal(HttpStatusCode.NotFound, arResponse.StatusCode);

                var content = await arResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Action found", content.Error);
            }
        }

        [Fact]
        public async Task CreateValidActionRelation()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newAction = await CreateTestAction();

                var newAR = new ActionRelation
                {
                    ActionId = newAction.Id,
                    Relationship = 0,
                    ConcernChange = new Matrix { X = 0, Y = 0 },
                    RewardResourceChange = new Matrix { X = 0, Y = 0 }
                };
                var arResponse = await client.PostAsJsonAsync("/api/actions/relations", newAR);
                Assert.Equal(HttpStatusCode.Created, arResponse.StatusCode);

                var action = await arResponse.Content.ReadAsJsonAsync<ActionRelation>();
                Assert.IsType(typeof(ActionRelation), action);
            }
        }

        [Fact]
        public async Task UpdateRewardByInvalidAction()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var actionForm = new Action { Verb = "invalidVerb" };

                var actionResponse = await client.PostAsJsonAsync("/api/actions/send", actionForm);
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var content = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Invalid action verb.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateRewardByValidAction()
        {
            var session = await Login();
            var newAction = await CreateTestAction("Verb test" + Guid.NewGuid());

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newAT = new AttributeType { Name = "testAttribute", DefaultValue = 0f, Type = 0 };
                var atResponse = await client.PostAsJsonAsync("/api/attributes/types", newAT);
                Assert.Equal(HttpStatusCode.Created, atResponse.StatusCode);
                var attribute = await atResponse.Content.ReadAsJsonAsync<AttributeType>();
                var newAR = new ActionRelation
                {
                    ActionId = newAction.Id,
                    Relationship = 0,
                    ConcernChange = new Matrix { X = 0, Y = 0 },
                    RewardResourceChange = new Matrix { X = 0, Y = 0 },
                };
                var arResponse = await client.PostAsJsonAsync("/api/actions/relations", newAR);
                Assert.Equal(HttpStatusCode.Created, arResponse.StatusCode);
                var arReturn = await arResponse.Content.ReadAsJsonAsync<ActionRelation>();
                Assert.IsType(typeof(ActionRelation), arReturn);
                var newReward = new Reward
                {
                    AttributeTypeId = attribute.Id,
                    TypeReward = RewardType.Store,
                    Value = 1.5f,
                    Status = 0,
                    GoalId = newAction.GoalId
                };
                var rewardResponse = await client.PostAsJsonAsync("/api/rewards", newReward);
                Assert.Equal(HttpStatusCode.Created, rewardResponse.StatusCode);
                var newReward2 = new Reward
                {
                    AttributeTypeId = attribute.Id,
                    TypeReward = RewardType.Modify,
                    Value = 1f,
                    Status = 0,
                    GoalId = newAction.GoalId,
                    ActionRelationId = arReturn.Id
                };
                var rewardResponse2 = await client.PostAsJsonAsync("/api/rewards", newReward2);
                Assert.Equal(HttpStatusCode.Created, rewardResponse2.StatusCode);

                var actionForm = new Action { Verb = newAction.Verb };

                var actionResponse = await client.PostAsJsonAsync("/api/actions/send", actionForm);
                Assert.Equal(HttpStatusCode.OK, actionResponse.StatusCode);

                var rewardReturn = await actionResponse.Content.ReadAsJsonAsync<Reward>();
                Assert.IsType(typeof(Reward), rewardReturn);
            }
        }

        [Fact]
        public async Task DeleteInvalidAction()
        {
            var session = await Login();
            var invalidId = new Guid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var actionResponse = await client.DeleteAsync($"/api/actions/{invalidId}");
                Assert.Equal(HttpStatusCode.NotFound, actionResponse.StatusCode);

                var content = await actionResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Action found.", content.Error);
            }
        }

        [Fact]
        public async Task DeleteValidAction()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newAction = await CreateTestAction();

                var actionForm = new Action
                {
                    Verb = "testerVerb",
                    ActivityId = newAction.ActivityId,
                    GoalId = newAction.GoalId
                };

                var actionResponse = await client.PostAsJsonAsync("/api/actions", actionForm);
                Assert.Equal(HttpStatusCode.Created, actionResponse.StatusCode);

                var action = await actionResponse.Content.ReadAsJsonAsync<Action>();
                Assert.IsType(typeof(Action), action);

                var actionResponse2 = await client.DeleteAsync($"/api/actions/{action.Id}");
                Assert.Equal(HttpStatusCode.OK, actionResponse2.StatusCode);

                var actions = await actionResponse2.Content.ReadAsJsonAsync<Action>();
                Assert.IsType(typeof(Action), actions);
            }
        }
    }
}