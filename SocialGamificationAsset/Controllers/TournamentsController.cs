using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/tournaments")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class TournamentsController : Controller
	{
		private SocialGamificationAssetContext _context;

		public TournamentsController(SocialGamificationAssetContext context)
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

		// GET: api/tournaments/all
		[HttpGet("all", Name = "GetAllTournaments")]
		public IEnumerable<Tournament> GetAllTournaments()
		{
			return _context.Tournaments;
		}

		// GET: api/tournaments/owned
		[HttpGet("owned", Name = "GetOwnedTournaments")]
		public async Task<IActionResult> GetOwnedTournaments()
		{
			IList<Tournament> tournaments = await _context.Tournaments
				.Where(t => t.OwnerId.Equals(session.Player.Id))
				.ToListAsync();

			if (tournaments == null || tournaments.Count() < 1)
			{
				return HttpNotFound("No Tournament Found.");
			}

			return Ok(tournaments);
		}

		// GET: api/tournaments
		// GET: api/tournaments/participated
		[HttpGet("", Name = "GetMyTournaments")]
		[HttpGet("participated", Name = "GetParticipatedTournaments")]
		public async Task<IActionResult> GetMyTournaments()
		{
			IList<Tournament> tournaments = await _context.Tournaments
				.Where(t => t.OwnerId.Equals(session.Player.Id))
				.ToListAsync();

			if (tournaments == null || tournaments.Count() < 1)
			{
				return HttpNotFound("No Tournament Found.");
			}

			return Ok(tournaments);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context.Dispose();
			}

			base.Dispose(disposing);
		}

		private bool TournamentExists(Guid id)
		{
			return _context.Tournaments.Count(e => e.Id.Equals(id)) > 0;
		}
	}
}
