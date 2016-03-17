using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using SocialGamificationAsset.Middlewares;
using SocialGamificationAsset.Models;

using Xunit;

namespace SocialGamificationAsset.Tests.Controllers
{
    public class ActivitiesControllerTest : ControllerTest
    {
        [Fact]
        public async Task AddActivityGoal_InvalidId()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var form = new UserForm();

                // Add Activity Goal with invalid Id
                var invalidId = Guid.NewGuid();
                var activityResponse = await client.PutAsJsonAsync($"/api/activities/{invalidId}/goal", form);
                Assert.Equal(HttpStatusCode.NotFound, activityResponse.StatusCode);

                var content = await activityResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("No such Activity found.", content.Error);
            }
        }

        [Fact]
        public async Task AddActivityGoal()
        {
            var session = await Login();
            var currentSeed = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id);

                var form = new Activity { Name = $"Name.{currentSeed}", Description = $"Description.{currentSeed}" };

                // Add Activity with valid data
                var activityResponse = await client.PostAsJsonAsync($"/api/activities", form);
                Assert.Equal(HttpStatusCode.Created, activityResponse.StatusCode);

                var activity = await activityResponse.Content.ReadAsJsonAsync<Activity>();
                Assert.Equal($"Name.{currentSeed}", activity.Name);

                var goalForm = new ActivityGoalForm { Description = $"Description.{currentSeed}" };
                activityResponse = await client.PutAsJsonAsync($"/api/activities/{activity.Id}/goal", goalForm);
                Assert.Equal(HttpStatusCode.OK, activityResponse.StatusCode);

                activity = await activityResponse.Content.ReadAsJsonAsync<Activity>();
                Assert.Equal(1, activity.Goals.Count);
            }
        }
    }
}