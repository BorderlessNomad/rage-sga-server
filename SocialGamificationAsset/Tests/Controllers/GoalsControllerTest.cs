using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
  public class GoalsControllerTest : ControllerTest
  {
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
    public async Task GetGoalByActivityInvalidActivity()
    {
      var session = await Login();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        // Get goal with Invalid activity Id
        var invalidId = Guid.NewGuid();
        var response = await client.GetAsync($"/api/goals/{invalidId}/activity");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var content = await response.Content.ReadAsJsonAsync<ApiError>();
        Assert.Equal($"No Goals found.", content.Error);
      }
    }

    [Fact]
    public async Task GetGoalByActivityValidActivity()
    {
      var role = await GetRole();
      var session = await Login();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        var goalRes = await client.GetAsync($"/api/goals/{role.ActivityId}/activity");
        Assert.Equal(HttpStatusCode.OK, goalRes.StatusCode);

        var goalGet = await goalRes.Content.ReadAsJsonAsync <List<Goal>>();
        Assert.IsType(typeof(List<Goal>), goalGet);
      }
    }

    [Fact]
    public async Task GetActorGoalWithoutSession()
    {
      var role = await GetRole();
      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson();

        var goalsResponse = await client.GetAsync($"/api/goals/{role.GoalId}/actor");
        Assert.Equal(HttpStatusCode.Unauthorized, goalsResponse.StatusCode);

        var fetched = await goalsResponse.Content.ReadAsJsonAsync<ApiError>();
        Assert.Equal($"No {SessionAuthorizeFilter.SessionHeaderName} Header found.", fetched.Error);
      }
    }

    [Fact]
    public async Task GetActorGoalWithInvalidSession()
    {
      var sessionId = "unknown";
      var role = await GetRole();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(sessionId);
        var goalsResponse = await client.GetAsync($"/api/goals/{role.GoalId}/actor");
        Assert.Equal(HttpStatusCode.Unauthorized, goalsResponse.StatusCode);

        var fetched = await goalsResponse.Content.ReadAsJsonAsync<ApiError>();
        Assert.Equal($"Invalid {SessionAuthorizeFilter.SessionHeaderName} Header.", fetched.Error);
      }
    }

    [Fact]
    public async Task GetActorGoalWithNonExistingSession()
    {
      var sessionId = Guid.NewGuid();
      var role = await GetRole();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(sessionId.ToString());
        var goalsResponse = await client.GetAsync($"/api/goals/{role.GoalId}/actor");
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
      var role = await GetRole();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        var goalsResponse = await client.GetAsync($"/api/goals/{role.GoalId}/actor");
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
      var role = await GetRole();
      var session = await Login();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        var goalsResponse = await client.GetAsync($"/api/goals/{role.GoalId}");
        Assert.Equal(HttpStatusCode.OK, goalsResponse.StatusCode);

        var goalGet = await goalsResponse.Content.ReadAsJsonAsync<Goal>();
        Assert.Equal(role.GoalId, goalGet.Id);
      }
    }

    [Fact]
    public async Task GetInvalidGoalDetailed()
    {
      var session = await Login();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        // Get 'detailed' goal with Invalid activity Id
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
      var role = await GetRole();
      var session = await Login();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        var goalsResponse = await client.GetAsync($"/api/goals/{role.GoalId}/detailed");
        Assert.Equal(HttpStatusCode.OK, goalsResponse.StatusCode);

        var goalGet = await goalsResponse.Content.ReadAsJsonAsync<Goal>();
        Assert.Equal(role.GoalId, goalGet.Id);
      }
    }

    [Fact]
    public async Task UpdateGoalInvalidGoal()
    {
      var role = await GetRole();
      var session = await Login();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());

        // Update Goal with Invalid Id
        var invalidId = Guid.NewGuid();
        var goalForm = new Goal { Description = "updated"};
        var goalUpdateResponse = await client.PutAsJsonAsync($"/api/goals/{invalidId}", goalForm);
        Assert.Equal(HttpStatusCode.NotFound, goalUpdateResponse.StatusCode);

        var content = await goalUpdateResponse.Content.ReadAsJsonAsync<ApiError>();
        Assert.Equal($"No such Goal found.", content.Error);
      }
    }

    [Fact]
    public async Task UpdateGoalValid()
    {
      var role = await GetRole();
      var session = await Login();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());

        // Update Goal with Valid Id
        var goalForm = new Goal { Description = "updated" };
        var goalUpdateResponse = await client.PutAsJsonAsync($"/api/goals/{role.GoalId}", goalForm);
        Assert.Equal(HttpStatusCode.OK, goalUpdateResponse.StatusCode);

        var content = await goalUpdateResponse.Content.ReadAsJsonAsync<Goal>();
        Assert.Equal(role.GoalId, content.Id);
        Assert.Equal("updated", content.Description);
      }
    }

    [Fact]
    public async Task CreateGoal()
    {
      var role = await GetRole();
      var session = await Login();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());

        var newGoal = new Goal
        {
          ConcernId = role.Goal.ConcernId,
          RewardResourceId = role.Goal.RewardResourceId,
          FeedbackId = role.Goal.FeedbackId
        };

        var goalResponse = await client.PostAsJsonAsync("/api/goals", newGoal);
        Assert.Equal(HttpStatusCode.Created, goalResponse.StatusCode);

        var goal = await goalResponse.Content.ReadAsJsonAsync<Goal>();
        Assert.Equal(0, goal.Concern.Coordinates.X);
        Assert.Equal(0, goal.Concern.Coordinates.Y);
      }
    }

    [Fact]
    public async Task CreateGoalInvalidFeedback()
    {
      var role = await GetRole();
      var session = await Login();
      var invalidId = Guid.NewGuid();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());

        var newGoal = new Goal
        {
          ConcernId = role.Goal.ConcernId,
          RewardResourceId = role.Goal.RewardResourceId,
          FeedbackId = invalidId
        };

        var goalResponse = await client.PostAsJsonAsync("/api/goals", newGoal);
        Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

        var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
        Assert.Equal($"No GoalFeedback found for the passed ID", content.Error);
      }
    }

    [Fact]
    public async Task CreateGoalInvalidConcern()
    {
      var role = await GetRole();
      var session = await Login();
      var invalidId = Guid.NewGuid();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());

        var newGoal = new Goal
        {
          ConcernId = role.Goal.ConcernId,
          RewardResourceId = invalidId,
          FeedbackId = role.Goal.FeedbackId
        };

        var goalResponse = await client.PostAsJsonAsync("/api/goals", newGoal);
        Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

        var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
        Assert.Equal($"No RewardResource found for the passed ID", content.Error);
      }
    }

    [Fact]
    public async Task CreateGoalInvalidRewardResource()
    {
      var role = await GetRole();
      var session = await Login();
      var invalidId = Guid.NewGuid();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());

        var newGoal = new Goal
        {
          ConcernId = invalidId,
          RewardResourceId = role.Goal.RewardResourceId,
          FeedbackId = role.Goal.FeedbackId
        };

        var goalResponse = await client.PostAsJsonAsync("/api/goals", newGoal);
        Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

        var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
        Assert.Equal($"No Concern found for the passed ID", content.Error);
      }
    }

    [Fact]
    public async Task CreateActorGoal()
    {
      var session = await Login();
      var role = await GetRole();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        var newGoal = new ActorGoal
        {
          ActorId = session.PlayerId,
          GoalId = role.GoalId,
          Status = 0,
          ConcernOutcomeId = role.Goal.ConcernId,
          RewardResourceOutcomeId = role.Goal.RewardResourceId,
          ActivityId = role.ActivityId,
          RoleId = role.Id
        };

        var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newGoal);
        Assert.Equal(HttpStatusCode.Created, goalResponse.StatusCode);

        var actorgoal = await goalResponse.Content.ReadAsJsonAsync<ActorGoal>();
        Assert.Equal(0, actorgoal.Goal.Concern.Coordinates.X);
        Assert.Equal(role.Description, actorgoal.Role.Description);
        Assert.Equal(role.Goal.Description, actorgoal.Goal.Description);
      }
    }

    [Fact]
    public async Task CreateActorGoalInvalidActivity()
    {
      var session = await Login();
      var invalidId = Guid.NewGuid();
      var role = await GetRole();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        var newGoal = new ActorGoal
        {
          ActorId = session.PlayerId,
          GoalId = role.GoalId,
          Status = 0,
          ConcernOutcomeId = role.Goal.ConcernId,
          RewardResourceOutcomeId = role.Goal.RewardResourceId,
          ActivityId = invalidId,
          RoleId = role.Id
        };

        var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newGoal);
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
      var role = await GetRole();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        var newGoal = new ActorGoal
        {
          ActorId = session.PlayerId,
          GoalId = role.GoalId,
          Status = 0,
          ConcernOutcomeId = role.Goal.ConcernId,
          RewardResourceOutcomeId = role.Goal.RewardResourceId,
          ActivityId = role.ActivityId,
          RoleId = invalidId
        };

        var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newGoal);
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
      var role = await GetRole();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        var newGoal = new ActorGoal
        {
          ActorId = session.PlayerId,
          GoalId = role.GoalId,
          Status = 0,
          ConcernOutcomeId = invalidId,
          RewardResourceOutcomeId = role.Goal.RewardResourceId,
          ActivityId = role.ActivityId,
          RoleId = role.Id
        };

        var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newGoal);
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
      var role = await GetRole();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        var newGoal = new ActorGoal
        {
          ActorId = session.PlayerId,
          GoalId = role.GoalId,
          Status = 0,
          ConcernOutcomeId = role.Goal.ConcernId,
          RewardResourceOutcomeId = invalidId,
          ActivityId = role.ActivityId,
          RoleId = role.Id
        };

        var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newGoal);
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
      var role = await GetRole();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        var newGoal = new ActorGoal
        {
          ActorId = session.PlayerId,
          GoalId = invalidId,
          Status = 0,
          ConcernOutcomeId = role.Goal.ConcernId,
          RewardResourceOutcomeId = role.Goal.RewardResourceId,
          ActivityId = role.ActivityId,
          RoleId = role.Id
        };

        var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newGoal);
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
      var role = await GetRole();

      using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
      {
        client.AcceptJson().AddSessionHeader(session.Id.ToString());
        var newGoal = new ActorGoal
        {
          ActorId = invalidId,
          GoalId = role.GoalId,
          Status = 0,
          ConcernOutcomeId = role.Goal.ConcernId,
          RewardResourceOutcomeId = role.Goal.RewardResourceId,
          ActivityId = role.ActivityId,
          RoleId = role.Id
        };

        var goalResponse = await client.PostAsJsonAsync("/api/goals/actors", newGoal);
        Assert.Equal(HttpStatusCode.NotFound, goalResponse.StatusCode);

        var content = await goalResponse.Content.ReadAsJsonAsync<ApiError>();
        Assert.Equal($"No Actor found for the passed ID", content.Error);
      }
    }
  }
}
