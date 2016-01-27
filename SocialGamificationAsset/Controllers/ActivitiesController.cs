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
	[Route("api/activities")]
	public class ActivitiesController : ApiController
	{
		public ActivitiesController(SocialGamificationAssetContext context) : base(context)
		{
		}

		// GET: api/activities
		[HttpGet]
		public IEnumerable<Activity> GetActivity()
		{
			return _context.Activities;
		}

		// GET: api/activities/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id}", Name = "GetActivity")]
		public async Task<IActionResult> GetActivity([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Activity test = await _context.Activities.FindAsync(id);

			if (test == null)
			{
				return HttpNotFound();
			}

			return Ok(test);
		}

		// PUT: api/activities/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id}")]
		public async Task<IActionResult> PutActivity([FromRoute] Guid id, [FromBody] Activity test)
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
				if (!ActivityExists(id))
				{
					return HttpNotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetActivity", new { id = test.Id }, test);
		}

		// POST: api/activities
		[HttpPost]
		[ResponseType(typeof(Activity))]
		public async Task<IActionResult> PostActivity([FromBody] Activity test)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.Activities.Add(test);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (ActivityExists(test.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetActivity", new { id = test.Id }, test);
		}

		// DELETE: api/activities/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteActivity([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Activity test = await _context.Activities.FindAsync(id);
			if (test == null)
			{
				return HttpNotFound();
			}

			_context.Activities.Remove(test);
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

		private bool ActivityExists(Guid id)
		{
			return _context.Activities.Count(e => e.Id == id) > 0;
		}
	}
}
