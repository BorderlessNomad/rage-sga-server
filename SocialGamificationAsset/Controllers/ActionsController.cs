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
	public class ActionsController : ApiController
	{
		public ActionsController(SocialGamificationAssetContext context) : base(context)
		{
		}

		// GET: api/actions
		[HttpGet]
		public IEnumerable<Models.Action> GetAction()
		{
			return _context.Actions;
		}

		// GET: api/actions/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id}", Name = "GetAction")]
		public async Task<IActionResult> GetAction([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Models.Action test = await _context.Actions.FindAsync(id);

			if (test == null)
			{
				return HttpNotFound();
			}

			return Ok(test);
		}

		// PUT: api/actions/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id}")]
		public async Task<IActionResult> PutAction([FromRoute] Guid id, [FromBody] Models.Action test)
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
				if (!ActionExists(id))
				{
					return HttpNotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetAction", new { id = test.Id }, test);
		}

		// POST: api/actions
		[HttpPost]
		[ResponseType(typeof(Models.Action))]
		public async Task<IActionResult> PostAction([FromBody] Models.Action test)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.Actions.Add(test);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (ActionExists(test.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetAction", new { id = test.Id }, test);
		}

		// DELETE: api/actions/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAction([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Models.Action test = await _context.Actions.FindAsync(id);
			if (test == null)
			{
				return HttpNotFound();
			}

			_context.Actions.Remove(test);
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

		private bool ActionExists(Guid id)
		{
			return _context.Actions.Count(e => e.Id == id) > 0;
		}
	}
}
