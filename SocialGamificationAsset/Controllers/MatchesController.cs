using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGAControllers.Controllers
{
	[Produces("application/json")]
	[Route("api/matches")]
	public class MatchesController : Controller
	{
		private SocialGamificationAssetContext _context;

		public MatchesController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		// GET: api/matches
		[HttpGet]
		public IEnumerable<Match> GetMatches()
		{
			return _context.Matches;
		}

		// GET: api/matches/5
		[HttpGet("{id}", Name = "GetMatch")]
		public async Task<IActionResult> GetMatch([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Match match = await _context.Matches.SingleAsync(m => m.Id == id);

			if (match == null)
			{
				return HttpNotFound();
			}

			return Ok(match);
		}

		// PUT: api/matches/5
		[HttpPut("{id}")]
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

		// POST: api/matches
		[HttpPost]
		public async Task<IActionResult> PostMatch([FromBody] Match match)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.Matches.Add(match);
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

			return CreatedAtRoute("GetMatch", new { id = match.Id }, match);
		}

		// DELETE: api/matches/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMatch([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Match match = await _context.Matches.SingleAsync(m => m.Id == id);
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
