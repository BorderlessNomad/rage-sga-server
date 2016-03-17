using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Middlewares;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class GoalsControllerTest : ControllerTest
    {
        protected async Task<Goal> CreateTestGoal()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var goal = new Goal
                {
                    Concern = new ConcernMatrix { Coordinates = new Matrix { X = 1, Y = 1 }, Category = 0 },
                    RewardResource =
                        new RewardResourceMatrix { Coordinates = new Matrix { X = 2, Y = 2 }, Category = 0 },
                    Feedback = new GoalFeedback { Threshold = 0, Target = 0, Direction = 0 },
                    Description = "Created for test cases"
                };
                var goalResponse = await client.PostAsJsonAsync("/api/goals", goal);
                Assert.Equal(HttpStatusCode.Created, goalResponse.StatusCode);

                var created = await goalResponse.Content.ReadAsJsonAsync<Goal>();

                return created;
            }
        }

        protected async Task<Role> CreateTestRole()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var role = new Role
                {
                    Name = "Testing" + Guid.NewGuid(),
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
                var roleResponse = await client.PostAsJsonAsync("/api/roles", role);
                Assert.Equal(HttpStatusCode.Created, roleResponse.StatusCode);

                var created = await roleResponse.Content.ReadAsJsonAsync<Role>();

                return created;
            }
        }

        [Fact]
        public async Task GetGoalsWithoutSession()
        {
            using (var client = _server.AcceptJson())
            {
                // Get goals without session header
                var goalsResponse = await client.GetAsync($"/api/goals");
                Assert.Equal(HttpStatusCode.Unauthorized, goalsResponse.StatusCode);

                var fetched = await goalsResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No {SessionAuthorizeFilter.SessionHeaderName} Header found.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetGoalsWithInvalidSession()
        {
            var sessionId = "unknown";

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId);

                // Get goals with invalid session header
                var goalsResponse = await client.GetAsync("/api/goals");
                Assert.Equal(HttpStatusCode.Unauthorized, goalsResponse.StatusCode);

                var fetched = await goalsResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Invalid {SessionAuthorizeFilter.SessionHeaderName} Header.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetGoalsWithNonExistingSession()
        {
            var sessionId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId.ToString());

                // get goals with non-existing session header
                var goalsResponse = await client.GetAsync("/api/goals");
                Assert.Equal(HttpStatusCode.NotFound, goalsResponse.StatusCode);

                var fetched = await goalsResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Session {sessionId} is Invalid.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetGoalByRoleInvalidRole()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get goal with Invalid role Id
                var invalidName = Guid.NewGuid();
                var response = await client.GetAsync($"/api/goals/{invalidName}/role");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

                var content = await response.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Role found for the passed name", content.Error);
            }
        }

        [Fact]
        public async Task GetGoalByRoleValidRole()
        {
            var role = await CreateTestRole();
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var goalRes = await client.GetAsync($"/api/goals/{role.Name}/role");
                Assert.Equal(HttpStatusCode.OK, goalRes.StatusCode);

                var goalGet = await goalRes.Content.ReadAsJsonAsync<List<Goal>>();
                Assert.IsType(typeof(List<Goal>), goalGet);
            }
        }

        [Fact]
        public async Task GetActorGoalWithoutSession()
        {
            var newGoal = await CreateTestGoal();
            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson();

                var goalsResponse = await client.GetAsync($"/api/goals/{newGoal.Id}/actor");
                Assert.Equal(HttpStatusCode.Unauthorized, goalsResponse.StatusCode);

                var fetched = await goalsResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No {SessionAuthorizeFilter.SessionHeaderName} Header found.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetActorGoalWithInvalidSession()
        {
            var sessionId = "unknown";
            var newGoal = await CreateTestGoal();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId);
                var goalsResponse = await client.GetAsync($"/api/goals/{newGoal.Id}/actor");
                Assert.Equal(HttpStatusCode.Unauthorized, goalsResponse.StatusCode);

                var fetched = await goalsResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Invalid {SessionAuthorizeFilter.SessionHeaderName} Header.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetActorGoalWithNonExistingSession()
        {
            var sessionId = Guid.NewGuid();
            var newGoal = await CreateTestGoal();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId.ToString());
                var goalsResponse = await client.GetAsync($"/api/goals/{newGoal.Id}/actor");
                Assert.Equal(HttpStatusCode.NotFound, goalsResponse.StatusCode);

                var fetched = await goalsResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Session {sessionId} is Invalid.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetActorGoalInvalidGoal()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get actorgoal with Invalid goal Id
                var invalidId = Guid.NewGuid();
                var response = await client.GetAsync($"/api/goals/{invalidId}/actor");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

                var content = await response.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No ActorGoal found.", content.Error);
            }
        }

        [Fact]
        public async Task GetActorGoalValidGoal()
        {
            var session = await Login();
            var role = await CreateTestRole();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var newerGoal = new ActorGoal
                {
                    ActorId = session.PlayerId,
                    GoalId = role.Goal.Id,
                    Status = 0,
                    ConcernOutcomeId = role.Goal.ConcernId.Value,
                    RewardResourceOutcomeId = role.Goal.RewardResourceId.Value,
                    ActivityId = role.ActivityId,
                    RoleId = role.Id
                };

                var actorgoalResponse = await client.PostAsJsonAsync("/api/goals/actors", newerGoal);
                Assert.Equal(HttpStatusCode.Created, actorgoalResponse.StatusCode);

                var goalsResponse = await client.GetAsync($"/api/goals/{role.Goal.Id}/actor");
                Assert.Equal(HttpStatusCode.OK, goalsResponse.StatusCode);

                var goalGet = await goalsResponse.Content.ReadAsJsonAsync<ActorGoal>();
                Assert.IsType(typeof(ActorGoal), goalGet);
            }
        }

        [Fact]
        public async Task GetInvalidGoal()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get goal with Invalid Id
                var invalidId = Guid.NewGuid();
                var response = await client.GetAsync($"/api/goals/{invalidId}");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

                var content = await response.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Goal found.", content.Error);
            }
        }

        [Fact]
        public async Task GetGoalValid()
        {
            var newGoal = await CreateTestGoal();
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var goalsResponse = await client.GetAsync($"/api/goals/{newGoal.Id}");
                Assert.Equal(HttpStatusCode.OK, goalsResponse.StatusCode);

                var goalGet = await goalsResponse.Content.ReadAsJsonAsync<Goal>();
                Assert.Equal(newGoal.Id, goalGet.Id);
            }
        }

        [Fact]
        public async Task GetInvalidGoalDetailed()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get 'detailed' goal with Invalid goal Id
                var invalidId = Guid.NewGuid();
                var response = await client.GetAsync($"/api/goals/{invalidId}/detailed");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

                var content = await response.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Goal found.", content.Error);
            }
        }

        [Fact]
        public async Task GetGoalValidDetailed()
        {
            var newGoal = await CreateTestGoal();
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var goalsResponse = await client.GetAsync($"/api/goals/{newGoal.Id}/detailed");
                Assert.Equal(HttpStatusCode.OK, goalsResponse.StatusCode);

                var goalGet = await goalsResponse.Content.ReadAsJsonAsync<Goal>();
                Assert.Equal(newGoal.Id, goalGet.Id);
            }
        }

        [Fact]
        public async Task UpdateGoalInvalidGoal()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Update Goal with Invalid Id
                var invalidId = Guid.NewGuid();
                var goalForm = new Goal { Description = "updated" };
                var goalUpdateResponse = await client.PutAsJsonAsync($"/api/goals/{invalidId}", goalForm);
                Assert.Equal(HttpStatusCode.NotFound, goalUpdateResponse.StatusCode);

                var content = await goalUpdateResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No such Goal found.", content.Error);
            }
        }

        [Fact]
        public async Task UpdateGoalValid()
        {
            var newGoal = await CreateTestGoal();
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Update Goal with Valid Id
                var goalForm = new Goal { Description = "updated" };
                var goalUpdateResponse = await client.PutAsJsonAsync($"/api/goals/{newGoal.Id}", goalForm);
                Assert.Equal(HttpStatusCode.OK, goalUpdateResponse.StatusCode);

                var content = await goalUpdateResponse.Content.ReadAsJsonAsync<Goal>();
                Assert.Equal(newGoal.Id, content.Id);
                Assert.Equal("updated", content.Description);
            }
        }

        [Fact]
        public async Task CreateGoal()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newGoal = new Goal
                {
                    Concern = new ConcernMatrix { Coordinates = new Matrix { X = 3, Y = 3 }, Category = 0 },
                    RewardResource =
                        new RewardResourceMatrix { Coordinates = new Matrix { X = 4, Y = 4 }, Category = 0 },
                    Feedback = new GoalFeedback { Threshold = 0, Target = 0, Direction = 0 },
                    Description = "Test case creation"
                };

                var goalResponse = await client.PostAsJsonAsync("/api/goals", newGoal);
                Assert.Equal(HttpStatusCode.Created, goalResponse.StatusCode);

                var goal = await goalResponse.Content.ReadAsJsonAsync<Goal>();
                Assert.Equal(3, goal.Concern.Coordinates.X);
                Assert.Equal(4, goal.RewardResource.Coordinates.Y);
            }
        }

        [Fact]
        public async Task CreateGoalInvalidFeedback()
        {
            var newGoal = await CreateTestGoal();
            var session = await Login();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newerGoal = new Goal
                {
                    ConcernId = newGoal.ConcernId.Value,
                    RewardResourceId = newGoal.RewardResourceId.Value,
                    FeedbackId = invalidId
                };

                var goalResponse = await client.PostAsJsonAsync("/api/goals", newerGoal);
                Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

                var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No GoalFeedback found for the passed ID", content.Error);
            }
        }

        [Fact]
        public async Task CreateGoalInvalidConcern()
        {
            var newGoal = await CreateTestGoal();
            var session = await Login();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newerGoal = new Goal
                {
                    ConcernId = newGoal.ConcernId.Value,
                    RewardResourceId = invalidId,
                    FeedbackId = newGoal.FeedbackId
                };

                var goalResponse = await client.PostAsJsonAsync("/api/goals", newerGoal);
                Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

                var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No RewardResource found for the passed ID", content.Error);
            }
        }

        [Fact]
        public async Task CreateGoalInvalidRewardResource()
        {
            var newGoal = await CreateTestGoal();
            var session = await Login();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                var newerGoal = new Goal
                {
                    ConcernId = invalidId,
                    RewardResourceId = newGoal.RewardResourceId.Value,
                    FeedbackId = newGoal.FeedbackId
                };

                var goalResponse = await client.PostAsJsonAsync("/api/goals", newerGoal);
                Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

                var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Concern found for the passed ID", content.Error);
            }
        }

        [Fact]
        public async Task CreateActorGoal()
        {
            var session = await Login();
            var role = await CreateTestRole();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var newerGoal = new ActorGoal
                {
                    ActorId = session.PlayerId,
                    GoalId = role.Goal.Id,
                    Status = 0,
                    ConcernOutcomeId = role.Goal.ConcernId.Value,
                    RewardResourceOutcomeId = role.Goal.RewardResourceId.Value,
                    ActivityId = role.ActivityId,
                    RoleId = role.Id
                };

                var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newerGoal);
                Assert.Equal(HttpStatusCode.Created, goalResponse.StatusCode);

                var actorgoal = await goalResponse.Content.ReadAsJsonAsync<ActorGoal>();
                Assert.Equal(1, actorgoal.Goal.Concern.Coordinates.X);
                Assert.Equal(role.Description, actorgoal.Role.Description);
                Assert.Equal(role.Goal.Description, actorgoal.Goal.Description);
            }
        }

        [Fact]
        public async Task CreateActorGoalInvalidActivity()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();
            var role = await CreateTestRole();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var newerGoal = new ActorGoal
                {
                    ActorId = session.PlayerId,
                    GoalId = role.Goal.Id,
                    Status = 0,
                    ConcernOutcomeId = role.Goal.ConcernId.Value,
                    RewardResourceOutcomeId = role.Goal.RewardResourceId.Value,
                    ActivityId = invalidId,
                    RoleId = role.Id
                };

                var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newerGoal);
                Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

                var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Activity found for the passed ID", content.Error);
            }
        }

        [Fact]
        public async Task CreateActorGoalInvalidRole()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();
            var role = await CreateTestRole();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var newerGoal = new ActorGoal
                {
                    ActorId = session.PlayerId,
                    GoalId = role.Goal.Id,
                    Status = 0,
                    ConcernOutcomeId = role.Goal.ConcernId.Value,
                    RewardResourceOutcomeId = role.Goal.RewardResourceId.Value,
                    ActivityId = role.ActivityId,
                    RoleId = invalidId
                };

                var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newerGoal);
                Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

                var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Role found for the passed ID", content.Error);
            }
        }

        [Fact]
        public async Task CreateActorGoalInvalidConcern()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();
            var role = await CreateTestRole();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var newerGoal = new ActorGoal
                {
                    ActorId = session.PlayerId,
                    GoalId = role.Goal.Id,
                    Status = 0,
                    ConcernOutcomeId = invalidId,
                    RewardResourceOutcomeId = role.Goal.RewardResourceId.Value,
                    ActivityId = role.ActivityId,
                    RoleId = role.Id
                };

                var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newerGoal);
                Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

                var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No ConcernOutcome found for the passed ID", content.Error);
            }
        }

        [Fact]
        public async Task CreateActorGoalInvalidRewardResource()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();
            var role = await CreateTestRole();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var newerGoal = new ActorGoal
                {
                    ActorId = session.PlayerId,
                    GoalId = role.Goal.Id,
                    Status = 0,
                    ConcernOutcomeId = role.Goal.ConcernId.Value,
                    RewardResourceOutcomeId = invalidId,
                    ActivityId = role.ActivityId,
                    RoleId = role.Id
                };

                var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newerGoal);
                Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

                var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No RewardResourceOutcome found for the passed ID", content.Error);
            }
        }

        [Fact]
        public async Task CreateActorGoalInvalidGoal()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();
            var role = await CreateTestRole();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var newerGoal = new ActorGoal
                {
                    ActorId = session.PlayerId,
                    GoalId = invalidId,
                    Status = 0,
                    ConcernOutcomeId = role.Goal.ConcernId.Value,
                    RewardResourceOutcomeId = role.Goal.RewardResourceId.Value,
                    ActivityId = role.ActivityId,
                    RoleId = role.Id
                };

                var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newerGoal);
                Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

                var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Goal found for the passed ID", content.Error);
            }
        }

        [Fact]
        public async Task CreateActorGoalInvalidActor()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();
            var role = await CreateTestRole();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());
                var newerGoal = new ActorGoal
                {
                    ActorId = invalidId,
                    GoalId = role.Goal.Id,
                    Status = 0,
                    ConcernOutcomeId = role.Goal.ConcernId.Value,
                    RewardResourceOutcomeId = role.Goal.RewardResourceId.Value,
                    ActivityId = role.ActivityId,
                    RoleId = role.Id
                };

                var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newerGoal);
                Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

                var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Actor found for the passed ID", content.Error);
            }
        }

        [Fact]
        public async Task DeleteGoalInvalidGoal()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Update Goal with Invalid Id
                var invalidId = Guid.NewGuid();
                var goalForm = new Goal { Description = "updated" };
                var goalResponse = await client.DeleteAsync($"/api/goals/{invalidId}");
                Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

                var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Goal found.", content.Error);
            }
        }

        [Fact]
        public async Task DeleteGoalValidGoal()
        {
            var newGoal = await CreateTestGoal();
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var response = await client.DeleteAsync($"/api/goals/{newGoal.Id}");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var finishedGoal = await response.Content.ReadAsJsonAsync<Goal>();
                Assert.True(finishedGoal.IsDeleted);
            }
        }
    }
}