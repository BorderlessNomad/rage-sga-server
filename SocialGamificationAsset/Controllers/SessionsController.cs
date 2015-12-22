using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/sessions")]
	public class SessionsController : Controller
	{
		private SocialGamificationAssetContext _context;

		public SessionsController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		// GET: api/sessions
		[HttpGet]
		public IEnumerable<Session> GetSessions()
		{
			return _context.Sessions;
		}

		// GET: api/sessions/5
		[HttpGet("{id}", Name = "GetSession")]
		public async Task<IActionResult> GetSession([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Session session = await _context.Sessions.SingleAsync(m => m.Id == id);

			if (session == null)
			{
				return HttpNotFound();
			}

			return Ok(session);
		}

		// PUT: api/sessions/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutSession([FromRoute] Guid id, [FromBody] Session session)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (id != session.Id)
			{
				return HttpBadRequest();
			}

			_context.Entry(session).State = System.Data.Entity.EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!SessionExists(id))
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

		// POST: api/sessions
		[HttpPost]
		public async Task<IActionResult> PostSession([FromBody] Session session)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.Sessions.Add(session);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (SessionExists(session.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetSession", new { id = session.Id }, session);
		}

		// DELETE: api/sessions/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteSession([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Session session = await _context.Sessions.SingleAsync(m => m.Id == id);
			if (session == null)
			{
				return HttpNotFound();
			}

			_context.Sessions.Remove(session);
			await _context.SaveChangesAsync();

			return Ok(session);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool SessionExists(Guid id)
		{
			return _context.Sessions.Count(e => e.Id == id) > 0;
		}
	}
}
