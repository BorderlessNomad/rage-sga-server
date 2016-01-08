using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/actors")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class ActorsController : Controller
	{
		private SocialGamificationAssetContext _context;

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

		public ActorsController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		// GET: api/actors/whoami
		[HttpGet]
		[Route("whoami")]
		public async Task<IActionResult> WhoAmI()
		{
			if (session != null && session.Actor != null)
			{
				return Ok(session.Actor);
			}

			return HttpNotFound();
		}

		// GET: api/actors
		[HttpGet]
		public async Task<IActionResult> GetActors()
		{
			return Ok(_context.Actors.ToList());
		}

		// GET: api/actors/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id:Guid}", Name = "GetActor")]
		public async Task<IActionResult> GetActor([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Actor actor = await _context.Actors.FindAsync(id);

			if (actor == null)
			{
				return HttpBadRequest("Invalid ActorId");
			}

			return Ok(actor);
		}

		// PUT: api/actors/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id:Guid}")]
		public async Task<IActionResult> PutActor([FromRoute] Guid id, [FromBody] Actor actor)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (id != actor.Id)
			{
				return HttpBadRequest();
			}

			_context.Entry(actor).State = System.Data.Entity.EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ActorExists(id))
				{
					return HttpBadRequest("Invalid ActorId");
				}
				else
				{
					throw;
				}
			}

			return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
		}

		// POST: api/actors
		[HttpPost]
		public async Task<IActionResult> PostActor([FromBody] Actor actor)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.Actors.Add(actor);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (ActorExists(actor.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetActor", new { id = actor.Id }, actor);
		}

		// DELETE: api/actors/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id:Guid}")]
		public async Task<IActionResult> DeleteActor([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Actor actor = await _context.Actors.FindAsync(id);
			if (actor == null)
			{
				return HttpBadRequest("Invalid ActorId");
			}

			_context.Actors.Remove(actor);
			await _context.SaveChangesAsync();

			return Ok(actor);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool ActorExists(Guid id)
		{
			return _context.Actors.Count(e => e.Id == id) > 0;
		}
	}
}
