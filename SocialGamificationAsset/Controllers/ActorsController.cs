using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialGamificationAsset.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/actors")]
	public class ActorsController : Controller
	{
		private SocialGamificationAssetContext _context;

		public ActorsController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		// GET: api/actors/5
		[HttpGet("{id}", Name = "GetActor")]
		public async Task<IActionResult> GetActor([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Actor actor = await _context.Actors.SingleAsync(m => m.Id == id);

			if (actor == null)
			{
				return HttpNotFound();
			}

			return Ok(actor);
		}

		// PUT: api/actors/5
		[HttpPut("{id}")]
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
					return HttpNotFound();
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

		// DELETE: api/actors/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteActor([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Actor actor = await _context.Actors.SingleAsync(m => m.Id == id);
			if (actor == null)
			{
				return HttpNotFound();
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
