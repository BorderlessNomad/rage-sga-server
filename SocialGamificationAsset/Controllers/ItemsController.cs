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
	[Route("api/items")]
	public class ItemsController : ApiController
	{
		public ItemsController(SocialGamificationAssetContext context) : base(context)
		{
		}

		// GET: api/items
		[HttpGet]
		public async Task<IList<Item>> GetItems()
		{
			IList<Item> items = await _context.Items.Where(i => i.ActorId.Equals(session.Player.Id)).Include(i => i.Type).ToListAsync();

			return items;
		}

		// GET: api/items/types
		[HttpGet]
		[HttpGet("types", Name = "GetItemTypes")]
		public IEnumerable<ItemType> GetItemTypes()
		{
			// TODO: Return Item Types defined for the Game
			return _context.ItemTypes;
		}

		// GET: api/items/types/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet]
		[HttpGet("types/{id:Guid}", Name = "GetItemsByType")]
		public async Task<IActionResult> GetItemsByType([FromRoute] Guid id)
		{
			// TODO: Return ItemType with Count

			var results = await _context.Items.Where(i => i.ItemTypeId.Equals(id)).GroupBy(i => i.ItemTypeId).ToListAsync();

			return Ok(results);
		}

		// GET: api/items/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id}", Name = "GetItem")]
		[ResponseType(typeof(Item))]
		public async Task<IActionResult> GetItem([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Item item = await _context.Items.Where(i => i.Id.Equals(id)).Include(i => i.Type).FirstOrDefaultAsync();

			if (item == null)
			{
				return HttpNotFound("No such Item found.");
			}

			return Ok(item);
		}

		// POST: api/items
		[HttpPost]
		[ResponseType(typeof(Item))]
		public async Task<IActionResult> AddItem([FromBody] ItemForm form)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Item item = new Item();
			ItemType itemType = await _context.ItemTypes.Where(i => i.Id.Equals(form.ItemTypeId)).FirstOrDefaultAsync();
			if (itemType == null)
			{
				return HttpBadRequest("Invalid Item Type.");
			}

			item.Type = itemType;

			if (!form.ActorId.HasValue || form.ActorId == Guid.Empty)
			{
				item.Actor = session.Player;
			}

			if (!form.Quantity.HasValue || form.Quantity <= 0)
			{
				item.Quantity = 1;
			}
			else
			{
				item.Quantity = (int)form.Quantity;
			}

			_context.Items.Add(item);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (ItemExists(item.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetItem", new { id = item.Id }, item);
		}

		// DELETE: api/items/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteItem([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Item item = await _context.Items.FindAsync(id);
			if (item == null)
			{
				return HttpNotFound();
			}

			_context.Items.Remove(item);
			await _context.SaveChangesAsync();

			return Ok(item);
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
