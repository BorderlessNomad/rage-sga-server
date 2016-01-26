using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/matches")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class MatchesController : Controller
	{
		private SocialGamificationAssetContext _context;

		public MatchesController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		private Session _session;

		public Session session
		{
			get { return GetSession(); }
		}

		protected Session GetSession()
		{
			if (_session == null)
			{
				_session = HttpContext.Session.GetObjectFromJson<Session>("__session");
			}

			return _session;
		}

		// GET: api/matches/all
		[HttpGet("all", Name = "GetAllMatches")]
		public IEnumerable<Match> GetAllMatches()
		{
			return _context.Matches.Include(m => m.Tournament);
		}

		// GET: api/matches/owned
		[HttpGet("owned", Name = "GetOwnedMatches")]
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
		public async Task<IActionResult> GetMatchRounds([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			IList<MatchActor> matchActors = await _context.MatchActors.Where(a => a.MatchId.Equals(id)).Include(a => a.Actor).ToListAsync();

			IList<MatchRound> rounds = new List<MatchRound>();
			foreach (MatchActor matchActor in matchActors)
			{
				MatchRound round = await _context.MatchRounds.Where(r => r.MatchActorId.Equals(matchActor.Id)).FirstOrDefaultAsync();
				rounds.Add(round);
			}

			if (matchActors == null || matchActors.Count < 1)
			{
				return HttpNotFound("No Actor Found for Match " + id);
			}

			return Ok(rounds);
		}

		// GET: api/matches/936da01f-9abd-4d9d-80c7-02af85c822a8/owner
		[HttpGet("{id:Guid}/owner", Name = "GetMatchOwner")]
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
		[HttpPut("{id:Guid}")]
		public async Task<IActionResult> PutMatch([FromRoute] Guid id, [FromBody] Match match)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (id != match.Id)
			{
				return HttpBadRequest("Invalid Match Resource Identifier.");
			}

			_context.Entry(match).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (!MatchExists(id))
				{
					return HttpNotFound("No Match Found for ID " + id);
				}
				else
				{
					throw;
				}
			}

			return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
		}

		// Creates a Quick Match between logged account and a random user
		// POST: api/matches
		[HttpPost]
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
		[HttpDelete("{id:Guid}")]
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
