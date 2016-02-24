using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/matches")]
    public class MatchesController : ApiController
    {
        public MatchesController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/matches
        // GET: api/matches/participated
        [HttpGet("", Name = "GetMyMatches")]
        [HttpGet("participated", Name = "GetParticipatedMatches")]
        [ResponseType(typeof(IList<Match>))]
        public async Task<IActionResult> GetMyMatches()
        {
            IList<Match> matches =
                await
                _context.MatchActors.Where(a => a.ActorId.Equals(session.Player.Id))
                        .Select(m => m.Match)
                        .Include(m => m.Tournament)
                        .ToListAsync();

            return Ok(matches);
        }

        // GET: api/matches/owned
        [HttpGet("owned", Name = "GetOwnedMatches")]
        [ResponseType(typeof(IList<Match>))]
        public async Task<IActionResult> GetOwnedMatches()
        {
            IList<Match> matches =
                await
                _context.MatchActors.Where(a => a.ActorId.Equals(session.Player.Id))
                        .Select(m => m.Match)
                        .Include(m => m.Tournament)
                        .Where(m => m.Tournament.OwnerId.Equals(session.Player.Id))
                        .ToListAsync();

            return Ok(matches);
        }

        // GET: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id:Guid}", Name = "GetMatch")]
        [ResponseType(typeof(Match))]
        public async Task<IActionResult> GetMatch([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var match =
                await _context.Matches.Where(m => m.Id.Equals(id)).Include(m => m.Tournament).FirstOrDefaultAsync();

            if (match == null)
            {
                return Helper.HttpNotFound($"No Match found for ID {id}");
            }

            return Ok(match);
        }

        // GET: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8/actors
        [HttpGet("{id:Guid}/actors", Name = "GetMatchActors")]
        [ResponseType(typeof(IList<MatchActor>))]
        public async Task<IActionResult> GetMatchActors([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            IList<MatchActor> matchActors =
                await _context.MatchActors.Where(a => a.MatchId.Equals(id)).Include(a => a.Actor).ToListAsync();

            if (matchActors == null || matchActors.Count < 1)
            {
                return Helper.HttpNotFound($"No Actor found for Match {id}");
            }

            return Ok(matchActors);
        }

        // GET: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8/rounds
        [HttpGet("{id:Guid}/rounds", Name = "GetMatchRounds")]
        [ResponseType(typeof(IList<MatchRoundResponse>))]
        public async Task<IActionResult> GetMatchRounds([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var match = await _context.Matches.Where(m => m.Id.Equals(id)).Include(m => m.Actors).FirstOrDefaultAsync();
            if (match == null)
            {
                return Helper.HttpNotFound("No such Match found.");
            }

            IList<Guid> matchActors = match.Actors.AsEnumerable().Select(a => a.Id).ToList();

            IList<MatchRound> matchRounds =
                await
                _context.MatchRounds.Where(r => matchActors.Contains(r.MatchActorId))
                        .OrderBy(r => r.RoundNumber)
                        .ToListAsync();

            IList<MatchRoundResponse> matchRoundResponse = new List<MatchRoundResponse>();
            foreach (var round in matchRounds)
            {
                var pushRound = false;
                var mRound = matchRoundResponse.FirstOrDefault(r => r.RoundNumber.Equals(round.RoundNumber));
                if (mRound == null)
                {
                    mRound = new MatchRoundResponse
                    {
                        RoundNumber = round.RoundNumber,
                        Actors = new List<MatchRoundActor>()
                    };

                    pushRound = true;
                }

                var actor = match.Actors.FirstOrDefault(a => a.Id.Equals(round.MatchActorId));
                if (actor != null)
                {
                    var matchRoundActor = new MatchRoundActor
                    {
                        ActorId = actor.ActorId,
                        Score = round.Score,
                        DateScore = round.DateScore
                    };

                    mRound.Actors.Add(matchRoundActor);
                }

                if (pushRound)
                {
                    matchRoundResponse.Add(mRound);
                }
            }

            return Ok(matchRoundResponse);
        }

        // GET: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8/owner
        [HttpGet("{id:Guid}/owner", Name = "GetMatchOwner")]
        [ResponseType(typeof(Actor))]
        public async Task<IActionResult> GetMatchOwner([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var match =
                await
                _context.Matches.Where(m => m.Id.Equals(id)).Include(m => m.Tournament.Owner).FirstOrDefaultAsync();

            if (match == null)
            {
                return Helper.HttpNotFound($"No Match found for ID {id}");
            }

            return Ok(match.Tournament.Owner);
        }

        // PUT: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id:Guid}/rounds", Name = "UpdateMatchRoundScore")]
        [ResponseType(typeof(MatchRound))]
        public async Task<IActionResult> UpdateMatchRoundScore([FromRoute] Guid id, [FromBody] MatchRoundForm form)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var match =
                await
                _context.Matches.Where(m => m.Id.Equals(id) && m.IsFinished.Equals(false))
                        .Include(m => m.Actors)
                        .FirstOrDefaultAsync();
            if (match == null)
            {
                return Helper.HttpNotFound("No running Match found.");
            }

            foreach (var actor in match.Actors.Where(actor => actor.ActorId.Equals(form.ActorId)))
            {
                var round =
                    await
                    _context.MatchRounds.Where(r => r.MatchActorId.Equals(actor.Id))
                            .Where(r => r.RoundNumber.Equals(form.RoundNumber))
                            .FirstOrDefaultAsync();

                if (round == null)
                {
                    return Helper.HttpNotFound($"No Round #{form.RoundNumber} found for Actor {form.ActorId}");
                }

                _context.Entry(round).State = EntityState.Modified;
                round.Score = form.Score;
                round.DateScore = DateTime.Now;

                var error = await SaveChangesAsync();
                if (error != null)
                {
                    return error;
                }

                return Ok(round);
            }

            return Helper.HttpNotFound($"No Actor {form.ActorId} found for this Match.");
        }

        // PUT: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id:Guid}", Name = "UpdateMatch")]
        [ResponseType(typeof(MatchRound))]
        public async Task<IActionResult> UpdateMatch([FromRoute] Guid id, [FromBody] Match match)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            if (id != match.Id)
            {
                return Helper.HttpBadRequest("Invalid Match Id.");
            }

            _context.Entry(match).State = EntityState.Modified;

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetMatch", new { id = match.Id }, match);
        }

        // Creates a Quick Match between given actors
        // POST: api/matches/actors
        [HttpPost("actors", Name = "CreateQuickMatchActors")]
        [ResponseType(typeof(Match))]
        public async Task<IActionResult> CreateQuickMatch([FromBody] QuickMatchActors quickMatch)
        {
            if (session?.Player == null)
            {
                return Helper.HttpNotFound("No Session/Player found.");
            }

            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            if (quickMatch.Actors.Count < 2)
            {
                return Helper.HttpBadRequest("Minimum 2 Actors are required for a Match");
            }

            var result = new QuickMatchResult();

            IList<Player> players = new List<Player>();
            IList<Group> groups = new List<Group>();

            if (quickMatch.Type == MatchType.Player)
            {
                foreach (var actorId in quickMatch.Actors)
                {
                    var player = await _context.Players.FindAsync(actorId);
                    if (player == null)
                    {
                        return Helper.HttpNotFound($"No Player with Id {actorId} exists.");
                    }

                    players.Add(player);
                }

                result = await QuickMatch(quickMatch, players);
            }
            else
            {
                foreach (var actorId in quickMatch.Actors)
                {
                    var group = await _context.Groups.FindAsync(actorId);
                    if (group == null)
                    {
                        return Helper.HttpNotFound($"No Group with Id {actorId} exists.");
                    }

                    groups.Add(group);
                }

                result = await QuickMatch(quickMatch, groups);
            }

            if (result.error != null)
            {
                return result.error;
            }

            return CreatedAtRoute("GetMatch", new { id = result.match.Id }, result.match);
        }

        // Creates a Quick Match between logged account and a random user
        // POST: api/matches
        [HttpPost("", Name = "CreateQuickMatch")]
        [ResponseType(typeof(Match))]
        public async Task<IActionResult> CreateQuickMatch([FromBody] QuickMatch quickMatch)
        {
            if (session?.Player == null)
            {
                return Helper.HttpNotFound("No Session/Player found.");
            }

            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var result = new QuickMatchResult();

            // Build the filter by CustomData
            var customData = CustomDataBase.Parse(quickMatch.CustomData);

            if (quickMatch.Type == MatchType.Player)
            {
                var players =
                    await
                    Player.LoadRandom(
                        _context,
                        session.Player,
                        customData,
                        quickMatch.AlliancesOnly,
                        quickMatch.Actors - 1);
                players.Add(session.Player);

                if (players.Count < quickMatch.Actors)
                {
                    return Helper.HttpNotFound("No Players available for match at this moment.");
                }

                result = await QuickMatch(quickMatch, players);
            }
            else if (quickMatch.Type == MatchType.Group)
            {
                if (!quickMatch.ActorId.HasValue || quickMatch.ActorId == Guid.Empty)
                {
                    return Helper.HttpBadRequest("GroupId is required for Group Matches.");
                }

                var group = session.Player.Groups.FirstOrDefault(g => g.Id.Equals(quickMatch.ActorId));
                if (group == null)
                {
                    return Helper.HttpNotFound("No such Group found for Player.");
                }

                var groups =
                    await Group.LoadRandom(_context, group, customData, quickMatch.AlliancesOnly, quickMatch.Actors - 1);
                groups.Add(group);

                if (groups.Count < quickMatch.Actors)
                {
                    return Helper.HttpNotFound("No Groups available for match at this moment.");
                }

                result = await QuickMatch(quickMatch, groups);
            }

            if (result.error != null)
            {
                return result.error;
            }

            return CreatedAtRoute("GetMatch", new { id = result.match.Id }, result.match);
        }

        private async Task<QuickMatchResult> QuickMatch(QuickMatch quickMatch, IEnumerable<Player> players)
        {
            Tournament tournament;
            ContentResult error;
            if (quickMatch.Tournament.HasValue && quickMatch.Tournament != Guid.Empty)
            {
                tournament = await _context.Tournaments.FindAsync(quickMatch.Tournament);
                if (tournament == null)
                {
                    return new QuickMatchResult { match = null, error = Helper.HttpBadRequest("Invalid Tournament.") };
                }
            }
            else
            {
                tournament = new Tournament { OwnerId = session.Player.Id };

                _context.Tournaments.Add(tournament);

                error = await SaveChangesAsync();
                if (error != null)
                {
                    return new QuickMatchResult { match = null, error = error };
                }
            }

            // Create Match
            var match = new Match { TournamentId = tournament.Id, TotalRounds = quickMatch.Rounds };

            if (!string.IsNullOrWhiteSpace(quickMatch.Title) && Helper.SanitizeString(quickMatch.Title).Length > 0)
            {
                match.Title = Helper.SanitizeString(quickMatch.Title);
            }

            _context.Matches.Add(match);

            error = await SaveChangesAsync();
            if (error != null)
            {
                return new QuickMatchResult { match = match, error = error };
            }

            foreach (var actor in players)
            {
                error = await MatchActor.Add(_context, match, actor);
                if (error != null)
                {
                    return new QuickMatchResult { match = match, error = error };
                }
            }

            return new QuickMatchResult { match = match, error = null };
        }

        private async Task<QuickMatchResult> QuickMatch(QuickMatch quickMatch, IEnumerable<Group> groups)
        {
            Tournament tournament;
            ContentResult error;
            if (quickMatch.Tournament.HasValue && quickMatch.Tournament != Guid.Empty)
            {
                tournament = await _context.Tournaments.FindAsync(quickMatch.Tournament);
                if (tournament == null)
                {
                    return new QuickMatchResult { match = null, error = Helper.HttpBadRequest("Invalid Tournament.") };
                }
            }
            else
            {
                tournament = new Tournament { OwnerId = session.Player.Id };

                _context.Tournaments.Add(tournament);

                error = await SaveChangesAsync();
                if (error != null)
                {
                    return new QuickMatchResult { match = null, error = error };
                }
            }

            // Create Match
            var match = new Match { TournamentId = tournament.Id, TotalRounds = quickMatch.Rounds };

            _context.Matches.Add(match);

            error = await SaveChangesAsync();
            if (error != null)
            {
                return new QuickMatchResult { match = match, error = error };
            }

            foreach (var actor in groups)
            {
                error = await MatchActor.Add(_context, match, actor);
                if (error != null)
                {
                    return new QuickMatchResult { match = match, error = error };
                }
            }

            return new QuickMatchResult { match = match, error = null };
        }

        // DELETE: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id:Guid}", Name = "DeleteMatch")]
        [ResponseType(typeof(Match))]
        public async Task<IActionResult> DeleteMatch([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return Helper.HttpNotFound("No Match found.");
            }

            match.IsDeleted = true;

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return Ok(match);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}