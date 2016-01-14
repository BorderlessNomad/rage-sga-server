using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/alliances")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class AlliancesController : Controller
	{
		private SocialGamificationAssetContext _context;

		public AlliancesController(SocialGamificationAssetContext context)
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

		// GET: api/alliances/all
		[HttpGet]
		[Route("all", Name = "GetAllAlliances")]
		public async Task<IActionResult> GetAllAlliances()
		{
			if (session == null || session.Player == null)
			{
				return HttpNotFound("Invalid Session.");
			}

			IList<Actor> alliances = await session.Player.Alliances(_context).ToListAsync();

			return Ok(alliances);
		}

		// GET: api/alliances
		// GET: api/alliances/accepted
		// GET: api/alliances/pending
		// GET: api/alliances/declined
		[HttpGet]
		[Route("{state?}", Name = "GetMyAlliances")]
		public async Task<IActionResult> GetMyAlliances([FromRoute] string state = "accepted")
		{
			if (session == null || session.Player == null)
			{
				return HttpNotFound("Invalid Session.");
			}

			AllianceState allianceshipStatus = AllianceState.Accepted;
			if (state == "pending")
			{
				allianceshipStatus = AllianceState.Pending;
			}
			else if (state == "declined")
			{
				allianceshipStatus = AllianceState.Declined;
			}

			IList<Actor> alliances = await session.Player.Alliances(_context, allianceshipStatus).ToListAsync();

			return Ok(alliances);
		}

		// GET: api/alliances/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet]
		[Route("{actorId:Guid}", Name = "GetActorAlliances")]
		public async Task<IActionResult> GetActorAlliances([FromRoute] Guid actorId)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(actorId)).FirstOrDefaultAsync();

			if (actor == null)
			{
				return HttpNotFound("No Actor Found.");
			}

			IList<Actor> alliances = await actor.Alliances(_context).ToListAsync();

			return Ok(alliances);
		}

		// PUT: api/alliances/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut]
		[Route("{allianceId:Guid}")]
		public async Task<IActionResult> AddAlliance([FromRoute] Guid allianceId)
		{
			if (session == null || session.Player == null)
			{
				return HttpBadRequest("Error with your session.");
			}

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(allianceId)).FirstOrDefaultAsync();

			if (actor == null)
			{
				return HttpNotFound("No such Actor found.");
			}

			Alliance alliance = await _context.Alliances
				.Where(Alliance.IsAlliance(allianceId, session.Player.Id))
				.FirstOrDefaultAsync();

			if (alliance != null)
			{
				if (alliance.State != AllianceState.Accepted)
				{
					return HttpBadRequest("Alliance Request already sent.");
				}

				return HttpBadRequest("You are aleady alliance with this Actor.");
			}

			Alliance newAlliance = new Alliance()
			{
				RequesterId = session.Player.Id,
				RequesteeId = allianceId
			};

			_context.Alliances.Add(newAlliance);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException e)
			{
				throw e;
			}

			return Ok(newAlliance);
		}

		// Delete: api/alliances/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete]
		[Route("{allianceId:Guid}")]
		public async Task<IActionResult> Unalliance([FromRoute] Guid allianceId)
		{
			if (session == null || session.Player == null)
			{
				return HttpBadRequest("Error with your session.");
			}

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(allianceId)).FirstOrDefaultAsync();

			if (actor == null)
			{
				return HttpNotFound("No such Actor found.");
			}

			Alliance alliance = await _context.Alliances
				.Where(Alliance.IsAlliance(allianceId, session.Player.Id))
				.FirstOrDefaultAsync();

			if (alliance == null)
			{
				return HttpBadRequest("Alliance is not in list.");
			}

			_context.Alliances.Remove(alliance);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException e)
			{
				throw e;
			}

			return Ok("Alliance Removed fromt the list.");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context.Dispose();
			}

			base.Dispose(disposing);
		}

		private bool AllianceExists(Guid id)
		{
			return _context.Alliances.Count(e => e.Id == id) > 0;
		}
	}
}
