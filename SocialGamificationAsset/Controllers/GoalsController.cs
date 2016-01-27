using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace SocialGamificationAsset.Controllers
{
	[Route("api/goals")]
	public class GoalsController : ApiController
	{
		public GoalsController(SocialGamificationAssetContext context) : base(context)
		{
		}

		// GET: api/goals
		[HttpGet]
		public IEnumerable<Goal> GetGoal()
		{
			return _context.Goals;
		}

		// GET: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id}", Name = "GetGoal")]
		public async Task<IActionResult> GetGoal([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Goal test = await _context.Goals.FindAsync(id);

			if (test == null)
			{
				return HttpNotFound();
			}

			return Ok(test);
		}

		// PUT: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id}")]
		public async Task<IActionResult> PutGoal([FromRoute] Guid id, [FromBody] Goal test)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (id != test.Id)
			{
				return HttpBadRequest();
			}

			_context.Entry(test).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (!GoalExists(id))
				{
					return HttpNotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetGoal", new { id = test.Id }, test);
		}

		// POST: api/goals
		[HttpPost]
		[ResponseType(typeof(Goal))]
		public async Task<IActionResult> PostGoal([FromBody] Goal test)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.Goals.Add(test);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (GoalExists(test.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetGoal", new { id = test.Id }, test);
		}

		// DELETE: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteGoal([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Goal test = await _context.Goals.FindAsync(id);
			if (test == null)
			{
				return HttpNotFound();
			}

			_context.Goals.Remove(test);
			await _context.SaveChangesAsync();

			return Ok(test);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context.Dispose();
			}

			base.Dispose(disposing);
		}

		private bool GoalExists(Guid id)
		{
			return _context.Goals.Count(e => e.Id == id) > 0;
		}
	}
}
