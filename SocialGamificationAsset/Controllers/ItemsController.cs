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
using System.Web.Http.Description;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/items")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class ItemsController : Controller
	{
		private SocialGamificationAssetContext _context;

		public ItemsController(SocialGamificationAssetContext context)
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

		// GET: api/items
		[HttpGet]
		public IEnumerable<Item> GetItem()
		{
			return _context.Items;
		}

		// GET: api/items/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id}", Name = "GetItem")]
		public async Task<IActionResult> GetItem([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Item test = await _context.Items.FindAsync(id);

			if (test == null)
			{
				return HttpNotFound();
			}

			return Ok(test);
		}

		// PUT: api/items/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id}")]
		public async Task<IActionResult> PutItem([FromRoute] Guid id, [FromBody] Item test)
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
				if (!ItemExists(id))
				{
					return HttpNotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetItem", new { id = test.Id }, test);
		}

		// POST: api/items
		[HttpPost]
		[ResponseType(typeof(Item))]
		public async Task<IActionResult> PostItem([FromBody] Item test)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.Items.Add(test);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (ItemExists(test.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetItem", new { id = test.Id }, test);
		}

		// DELETE: api/items/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteItem([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Item test = await _context.Items.FindAsync(id);
			if (test == null)
			{
				return HttpNotFound();
			}

			_context.Items.Remove(test);
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

		private bool ItemExists(Guid id)
		{
			return _context.Items.Count(e => e.Id == id) > 0;
		}
	}
}
