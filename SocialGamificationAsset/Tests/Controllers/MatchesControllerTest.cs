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
    public class MatchesControllerTest : ControllerTest
    {
        [Fact]
        public async Task GetMyMatchesWithoutSession()
        {
            using (var client = _server.AcceptJson())
            {
                // get my matches without session header
                var matchesResponse = await client.GetAsync($"/api/matches");
                Assert.Equal(HttpStatusCode.Unauthorized, matchesResponse.StatusCode);

                var fetched = await matchesResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No {SessionAuthorizeFilter.SessionHeaderName} Header found.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetMyMatchesWithInvalidSession()
        {
            var sessionId = "unknown";

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId);

                // get my matches with invalid session header
                var matchesResponse = await client.GetAsync("/api/matches");
                Assert.Equal(HttpStatusCode.Unauthorized, matchesResponse.StatusCode);

                var fetched = await matchesResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Invalid {SessionAuthorizeFilter.SessionHeaderName} Header.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetMyMatchesWithNonExistingSession()
        {
            var sessionId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(sessionId.ToString());

                // get my matches with non-existing session header
                var matchesResponse = await client.GetAsync("/api/matches");
                Assert.Equal(HttpStatusCode.NotFound, matchesResponse.StatusCode);

                var fetched = await matchesResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"Session {sessionId} is Invalid.", fetched.Error);
            }
        }

        [Fact]
        public async Task GetParticipatedMatches()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get Matches that Player have participated in
                var matchesResponse = await client.GetAsync("/api/matches");
                Assert.Equal(HttpStatusCode.OK, matchesResponse.StatusCode);

                var matches = await matchesResponse.Content.ReadAsJsonAsync<IList<Match>>();
                Assert.IsType(typeof(List<Match>), matches);
            }
        }

        [Fact]
        public async Task GetOwnedMatches()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get Matches that Player own
                var matchesResponse = await client.GetAsync("/api/matches/owned");
                Assert.Equal(HttpStatusCode.OK, matchesResponse.StatusCode);

                var matches = await matchesResponse.Content.ReadAsJsonAsync<IList<Match>>();
                Assert.IsType(typeof(List<Match>), matches);

                foreach (var match in matches)
                {
                    Assert.True(match.Tournament.OwnerId.Equals(session.Player.Id));
                }
            }
        }

        [Fact]
        public async Task CreateQuickMatchActorsLessThan2Actors()
        {
            var mayur = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(mayur.Id.ToString());

                var quickMatch = new QuickMatchActors { Actors = new List<Guid>(new[] { mayur.Player.Id }) };

                var matchResponse = await client.PostAsJsonAsync("/api/matches/actors", quickMatch);
                Assert.Equal(HttpStatusCode.BadRequest, matchResponse.StatusCode);

                var content = await matchResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal("Minimum 2 Actors are required for a Match", content.Error);
            }
        }

        [Fact]
        public async Task CreateQuickMatchActorsInvalidActor()
        {
            var mayur = await Login();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(mayur.Id.ToString());

                var quickMatch = new QuickMatchActors { Actors = new List<Guid>(new[] { mayur.Player.Id, invalidId }) };

                var matchResponse = await client.PostAsJsonAsync("/api/matches/actors", quickMatch);
                Assert.Equal(HttpStatusCode.NotFound, matchResponse.StatusCode);

                var content = await matchResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Player with Id {invalidId} exists.", content.Error);
            }
        }

        [Fact]
        public async Task CreateQuickMatchActors()
        {
            var mayur = await Login();
            var matt = await Login("matt", "matt");

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(mayur.Id.ToString());

                var quickMatch = new QuickMatchActors
                {
                    Actors = new List<Guid>(new[] { mayur.Player.Id, matt.Player.Id })
                };

                var matchResponse = await client.PostAsJsonAsync("/api/matches/actors", quickMatch);
                Assert.Equal(HttpStatusCode.Created, matchResponse.StatusCode);

                var match = await matchResponse.Content.ReadAsJsonAsync<Match>();
                Assert.Equal(mayur.Player.Id, match.Tournament.OwnerId);
                Assert.False(match.IsFinished);
                Assert.False(match.IsDeleted);
            }
        }

        [Fact]
        public async Task GetMatchInvalidMatch()
        {
            var session = await Login();
            var invalidId = Guid.NewGuid();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get Match with Invalid Id
                var matchResponse = await client.GetAsync($"/api/matches/{invalidId}");
                Assert.Equal(HttpStatusCode.NotFound, matchResponse.StatusCode);

                var content = await matchResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No Match found with Id {invalidId}.", content.Error);
            }
        }

        [Fact]
        public async Task GetMatchValid()
        {
            var mayur = await Login();
            var matt = await Login("matt", "matt");

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(mayur.Id.ToString());

                var quickMatch = new QuickMatchActors
                {
                    Actors = new List<Guid>(new[] { mayur.Player.Id, matt.Player.Id })
                };

                var matchResponse = await client.PostAsJsonAsync("/api/matches/actors", quickMatch);
                Assert.Equal(HttpStatusCode.Created, matchResponse.StatusCode);

                var match = await matchResponse.Content.ReadAsJsonAsync<Match>();
                Assert.Equal(mayur.Player.Id, match.Tournament.OwnerId);
                Assert.False(match.IsFinished);
                Assert.False(match.IsDeleted);

                // Get Match with Valid Id
                matchResponse = await client.GetAsync($"/api/matches/{match.Id}");
                Assert.Equal(HttpStatusCode.OK, matchResponse.StatusCode);

                var matchGet = await matchResponse.Content.ReadAsJsonAsync<Match>();
                Assert.Equal(mayur.Player.Id, matchGet.Tournament.OwnerId);
                Assert.Equal(match.Id, matchGet.Id);
            }
        }

        [Fact]
        public async Task GetMatchActorsWithInvalidMatch()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get Match Actors with Invalid Id
                var invalidMatchId = Guid.NewGuid();
                var matchActorResponse = await client.GetAsync($"/api/matches/{invalidMatchId}/actors");
                Assert.Equal(HttpStatusCode.NotFound, matchActorResponse.StatusCode);

                var content = await matchActorResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No such Match found.", content.Error);
            }
        }

        [Fact]
        public async Task GetMatchActorsWithValidMatch()
        {
            var mayur = await Login();
            var matt = await Login("matt", "matt");

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(mayur.Id.ToString());

                var quickMatch = new QuickMatchActors
                {
                    Actors = new List<Guid>(new[] { mayur.Player.Id, matt.Player.Id })
                };

                var matchResponse = await client.PostAsJsonAsync("/api/matches/actors", quickMatch);
                Assert.Equal(HttpStatusCode.Created, matchResponse.StatusCode);

                var match = await matchResponse.Content.ReadAsJsonAsync<Match>();
                Assert.Equal(mayur.Player.Id, match.Tournament.OwnerId);
                Assert.False(match.IsFinished);
                Assert.False(match.IsDeleted);

                // Get Match with Valid Id
                matchResponse = await client.GetAsync($"/api/matches/{match.Id}");
                Assert.Equal(HttpStatusCode.OK, matchResponse.StatusCode);

                var matchGet = await matchResponse.Content.ReadAsJsonAsync<Match>();
                Assert.Equal(mayur.Player.Id, matchGet.Tournament.OwnerId);
                Assert.Equal(match.Id, matchGet.Id);

                // Get Match Actors with Valid Id
                var matchActorResponse = await client.GetAsync($"/api/matches/{match.Id}/actors");
                Assert.Equal(HttpStatusCode.OK, matchActorResponse.StatusCode);

                var matchActorGet = await matchActorResponse.Content.ReadAsStringAsync();
                var matchActors = JsonConvert.DeserializeObject<List<MatchActor>>(matchActorGet, Actor.JsonSerializerSettings());
                Assert.IsType(typeof(List<MatchActor>), matchActors);
            }
        }

        [Fact]
        public async Task GetMatchRoundsInvalidMatch()
        {
            var session = await Login();

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(session.Id.ToString());

                // Get Match Actors with Invalid Id
                var invalidMatchId = Guid.NewGuid();
                var matchRoundsResponse = await client.GetAsync($"/api/matches/{invalidMatchId}/rounds");
                Assert.Equal(HttpStatusCode.NotFound, matchRoundsResponse.StatusCode);

                var content = await matchRoundsResponse.Content.ReadAsJsonAsync<ApiError>();
                Assert.Equal($"No such Match found.", content.Error);
            }
        }

        [Fact]
        public async Task GetMatchRoundsWithValidMatch()
        {
            var mayur = await Login();
            var matt = await Login("matt", "matt");

            using (var client = new HttpClient { BaseAddress = new Uri(ServerUrl) })
            {
                client.AcceptJson().AddSessionHeader(mayur.Id.ToString());

                var quickMatch = new QuickMatchActors
                {
                    Actors = new List<Guid>(new[] { mayur.Player.Id, matt.Player.Id })
                };

                var matchResponse = await client.PostAsJsonAsync("/api/matches/actors", quickMatch);
                Assert.Equal(HttpStatusCode.Created, matchResponse.StatusCode);

                var match = await matchResponse.Content.ReadAsJsonAsync<Match>();
                Assert.Equal(mayur.Player.Id, match.Tournament.OwnerId);
                Assert.False(match.IsFinished);
                Assert.False(match.IsDeleted);

                // Get Match with Valid Id
                matchResponse = await client.GetAsync($"/api/matches/{match.Id}");
                Assert.Equal(HttpStatusCode.OK, matchResponse.StatusCode);

                var matchGet = await matchResponse.Content.ReadAsJsonAsync<Match>();
                Assert.Equal(mayur.Player.Id, matchGet.Tournament.OwnerId);
                Assert.Equal(match.Id, matchGet.Id);

                // Get Match Actors with Valid Id
                var matchRoundsResponse = await client.GetAsync($"/api/matches/{match.Id}/rounds");
                Assert.Equal(HttpStatusCode.OK, matchRoundsResponse.StatusCode);

                var matchRounds = await matchRoundsResponse.Content.ReadAsJsonAsync<List<MatchRound>>();
                Assert.IsType(typeof(List<MatchRound>), matchRounds);
            }
        }
    }
}