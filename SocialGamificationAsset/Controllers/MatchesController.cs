using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace SocialGamificationAsset.Controllers
{
	[Route("api/matches")]
	public class MatchesController : ApiController
	{
		public MatchesController(SocialGamificationAssetContext context) : base(context)
		{
		}

		// GET: api/matches/all
		[HttpGet("all", Name = "GetAllMatches")]
		[ResponseType(typeof(ItemTypeResponse))]
		public IEnumerable<Match> GetAllMatches()
		{
			return _context.Matches.Include(m => m.Tournament);
		}

		// GET: api/matches/owned
		[HttpGet("owned", Name = "GetOwnedMatches")]
		[ResponseType(typeof(IList<Match>))]
		public async Task<IActionResult> GetOwnedMatches()
		{
			IList<Match> matches = await _context.MatchActors
				.Where(a => a.ActorId.Equals(session.Player.Id))
				.Select(m => m.Match)
				.Include(m => m.Tournament)
				.Where(m => m.Tournament.OwnerId.Equals(session.Player.Id))
				.ToListAsync()
			;

			return Ok(matches);
		}

		// GET: api/matches
		// GET: api/matches/participated
		[HttpGet("", Name = "GetMyMatches")]
		[HttpGet("participated", Name = "GetParticipatedMatches")]
		[ResponseType(typeof(IList<Match>))]
		public async Task<IActionResult> GetMyMatches()
		{
			IList<Match> matches = await _context.MatchActors
				.Where(a => a.ActorId.Equals(session.Player.Id))
				.Select(m => m.Match)
				.Include(m => m.Tournament)
				.ToListAsync()
			;

			return Ok(matches);
		}

		// GET: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id:Guid}", Name = "GetMatch")]
		[ResponseType(typeof(Match))]
		public async Task<IActionResult> GetMatch([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Match match = await _context.Matches.Where(m => m.Id.Equals(id)).Include(m => m.Tournament).FirstOrDefaultAsync();

			if (match == null)
			{
				return HttpNotFound("No Match Found for ID " + id);
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
				return HttpBadRequest(ModelState);
			}

			IList<MatchActor> matchActors = await _context.MatchActors.Where(a => a.MatchId.Equals(id)).Include(a => a.Actor).ToListAsync();

			if (matchActors == null || matchActors.Count < 1)
			{
				return HttpNotFound("No Actor Found for Match " + id);
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
				return HttpBadRequest(ModelState);
			}

			Match match = await _context.Matches.Where(m => m.Id.Equals(id)).Include(m => m.Actors).FirstOrDefaultAsync();
			if (match == null)
			{
				return HttpNotFound("No such Match found.");
			}

			IList<Guid> matchActors = match.Actors.AsEnumerable().Select(a => a.Id).ToList();

			IList<MatchRound> matchRounds = await _context.MatchRounds
				.Where(r => matchActors.Contains(r.MatchActorId))
				.OrderBy(r => r.RoundNumber)
				.ToListAsync();

			IList<MatchRoundResponse> matchRoundResponse = new List<MatchRoundResponse>();
			foreach (MatchRound round in matchRounds)
			{
				bool pushRound = false;
				MatchRoundResponse mRound = matchRoundResponse.FirstOrDefault(r => r.RoundNumber.Equals(round.RoundNumber));
				if (mRound == null)
				{
					mRound = new MatchRoundResponse
					{
						RoundNumber = round.RoundNumber,
						Actors = new List<MatchRoundActor>()
					};

					pushRound = true;
				}

				MatchActor actor = match.Actors.FirstOrDefault(a => a.Id.Equals(round.MatchActorId));
				if (actor != null)
				{
					MatchRoundActor matchRoundActor = new MatchRoundActor
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
				return HttpBadRequest(ModelState);
			}

			Match match = await _context.Matches.Where(m => m.Id.Equals(id)).Include(m => m.Tournament.Owner).FirstOrDefaultAsync();

			if (match == null)
			{
				return HttpNotFound("No Match Found for ID " + id);
			}

			return Ok(match.Tournament.Owner);
		}

		// PUT: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id:Guid}/rounds", Name = "UpdateMatchRoundScore")]
		[ResponseType(typeof(MatchRound))]
		public async Task<IActionResult> UpdateMatchRoundScore([FromRoute] Guid id, [FromBody] MatchRoundForm roundForm)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Match match = await _context.Matches.Where(m => m.Id.Equals(id) && m.IsFinished.Equals(false)).Include(m => m.Actors).FirstOrDefaultAsync();
			if (match == null)
			{
				return HttpNotFound("No running Match found.");
			}

			foreach (MatchActor actor in match.Actors)
			{
				if (actor.ActorId.Equals(roundForm.ActorId))
				{
					MatchRound round = await _context.MatchRounds
						.Where(r => r.MatchActorId.Equals(actor.Id))
						.Where(r => r.RoundNumber.Equals(roundForm.RoundNumber))
						.FirstOrDefaultAsync();
					if (round == null)
					{
						return HttpNotFound("No Round #'" + roundForm.RoundNumber + "' found for Actor '" + roundForm.ActorId + "'");
					}

					_context.Entry(round).State = EntityState.Modified;
					round.Score = roundForm.Score;
					round.DateScore = DateTime.Now;

					try
					{
						await _context.SaveChangesAsync();
					}
					catch (DbUpdateException e)
					{
						throw e;
					}

					return Ok(round);
				}
			}

			return HttpNotFound("No Actor '" + roundForm.ActorId + "' found for this Match.");
		}

		// PUT: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id:Guid}", Name = "UpdateMatch")]
		[ResponseType(typeof(MatchRound))]
		public async Task<IActionResult> UpdateMatch([FromRoute] Guid id, [FromBody] Match match)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (id != match.Id)
			{
				return HttpBadRequest();
			}

			_context.Entry(match).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				if (!MatchExists(id))
				{
					return HttpNotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetMatch", new { id = match.Id }, match);
		}

		// Creates a Quick Match between logged account and a random user
		// POST: api/matches
		[HttpPost("", Name = "CreateQuickMatch")]
		[ResponseType(typeof(Match))]
		public async Task<IActionResult> CreateQuickMatch([FromBody] QuickMatch quickMatch)
		{
			if (session == null || session.Player == null)
			{
				return HttpNotFound();
			}

			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			// Build the filter by CustomData
			IList<CustomDataBase> customData = CustomDataBase.Parse(quickMatch.CustomData);
			IList<Player> players = new List<Player>();
			IList<Group> groups = new List<Group>();

			if (quickMatch.Type == MatchType.Player)
			{
				players = await Player.LoadRandom(_context, session.Player, customData, quickMatch.AlliancesOnly, quickMatch.Actors - 1);
				players.Add(session.Player);

				if (players.Count < quickMatch.Actors)
				{
					return HttpNotFound("No Players available for match at this moment.");
				}
			}
			else if (quickMatch.Type == MatchType.Group)
			{
				if (!quickMatch.ActorId.HasValue || quickMatch.ActorId == Guid.Empty)
				{
					return HttpBadRequest("GroupId is required for Group Matches.");
				}

				Group group = session.Player.Groups.FirstOrDefault(g => g.Id.Equals(quickMatch.ActorId));
				if (group == null)
				{
					return HttpNotFound("No such Group found for Player.");
				}

				groups = await Group.LoadRandom(_context, group, customData, quickMatch.AlliancesOnly, quickMatch.Actors - 1);
				groups.Add(group);

				if (groups.Count < quickMatch.Actors)
				{
					return HttpNotFound("No Groups available for match at this moment.");
				}
			}

			Tournament tournament;
			if (quickMatch.Tournament.HasValue && quickMatch.Tournament != Guid.Empty)
			{
				tournament = await _context.Tournaments.FindAsync(quickMatch.Tournament);
				if (tournament == null)
				{
					return HttpBadRequest("Invalid Tournament.");
				}
			}
			else
			{
				tournament = new Tournament()
				{
					OwnerId = session.Player.Id
				};

				_context.Tournaments.Add(tournament);

				try
				{
					await _context.SaveChangesAsync();
				}
				catch (DbEntityValidationException e)
				{
					throw e;
				}
			}

			// Create Match
			Match match = new Match()
			{
				TournamentId = tournament.Id,
				TotalRounds = quickMatch.Rounds
			};

			_context.Matches.Add(match);

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbEntityValidationException e)
			{
				if (MatchExists(match.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw e;
				}
			}

			if (quickMatch.Type == MatchType.Player)
			{
				foreach (Player actor in players)
				{
					MatchActor.Add(_context, match, actor);
				}
			}
			else
			{
				foreach (Group actor in groups)
				{
					MatchActor.Add(_context, match, actor);
				}
			}

			return CreatedAtRoute("GetMatch", new { id = match.Id }, match);
		}

		// DELETE: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id:Guid}", Name = "DeleteMatch")]
		[ResponseType(typeof(Match))]
		public async Task<IActionResult> DeleteMatch([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Match match = await _context.Matches.FindAsync(id);
			if (match == null)
			{
				return HttpNotFound();
			}

			match.IsDeleted = true;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (MatchExists(match.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
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

		private bool MatchExists(Guid id)
		{
			return _context.Matches.Count(e => e.Id.Equals(id)) > 0;
		}
	}
}
