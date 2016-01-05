using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace SGAControllers.Controllers
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

		// GET: api/matches
		[HttpGet]
		public IEnumerable<Match> GetMatches()
		{
			return _context.Matches;
		}

		// GET: api/matches/936DA01F-9ABD-4d9d-80C7-02AF85C822A8
		[HttpGet("{id:Guid}", Name = "GetMatch")]
		public async Task<IActionResult> GetMatch([FromRoute] Guid id)
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

			return Ok(match);
		}

		// PUT: api/matches/936DA01F-9ABD-4d9d-80C7-02AF85C822A8
		[HttpPut("{id:Guid}")]
		public async Task<IActionResult> PutMatch([FromRoute] Guid id, [FromBody] Match match)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (id != match.Id)
			{
				return HttpBadRequest();
			}

			_context.Entry(match).State = System.Data.Entity.EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
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

			return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
		}

		// Creates a Quick Match between logged account and a random user
		// POST: api/matches
		[HttpPost]
		public async Task<IActionResult> PostMatch([FromBody] QuickMatch quickMatch)
		{
			if (session == null || session.Actor == null)
			{
				return HttpNotFound();
			}

			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			IList<Actor> actors = actors = session.Actor.LoadRandom(_context, quickMatch.FriendsOnly, quickMatch.Actors);
			actors.Add(session.Actor);

			if (actors.Count() < quickMatch.Actors)
			{
				string verb = (quickMatch.Type == MatchType.Player) ? "Players" : "Groups";
				return HttpNotFound("No " + verb + " available for match at this moment.");
			}

			// Create Tournament
			Tournament tournament = new Tournament()
			{
				OwnerId = session.Actor.Id
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

			for (int i = 1; i <= match.TotalRounds; ++i)
			{
				foreach (Actor actor in actors)
				{
					// Add Round & Assign Actors
					MatchRound matchRound = new MatchRound()
					{
						MatchActor = new MatchActor()
						{
							MatchId = match.Id,
							ActorId = actor.Id
						}
					};

					_context.MatchRounds.Add(matchRound);

					try
					{
						await _context.SaveChangesAsync();
					}
					catch (DbEntityValidationException e)
					{
						throw e;
					}
				}
			}

			return CreatedAtRoute("GetMatch", new { id = match.Id }, match);
		}

		// DELETE: api/matches/936DA01F-9ABD-4d9d-80C7-02AF85C822A8
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

			_context.Matches.Remove(match);
			await _context.SaveChangesAsync();

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
			return _context.Matches.Count(e => e.Id == id) > 0;
		}
	}
}
