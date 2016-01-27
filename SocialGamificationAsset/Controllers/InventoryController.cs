using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	[Route("api/inventory")]
	public class InventoryController : ApiController
	{
		public InventoryController(SocialGamificationAssetContext context) : base(context)
		{
		}

		// GET: api/inventory
		[HttpGet]
		public IEnumerable<Inventory> GetInventory()
		{
			return _context.Inventory;
		}

		// GET: api/inventory/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id:Guid}", Name = "GetInventory")]
		public async Task<IActionResult> GetInventory([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Inventory inventory = await _context.Inventory.FindAsync(id);

			if (inventory == null)
			{
				return HttpNotFound();
			}

			return Ok(inventory);
		}

		// PUT: api/inventory/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id:Guid}")]
		public async Task<IActionResult> PutInventory([FromRoute] Guid id, [FromBody] Inventory inventory)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (id != inventory.Id)
			{
				return HttpBadRequest();
			}

			_context.Entry(inventory).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (!InventoryExists(id))
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

		// POST: api/inventory
		[HttpPost]
		public async Task<IActionResult> PostInventory([FromBody] Inventory inventory)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.Inventory.Add(inventory);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (InventoryExists(inventory.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetInventory", new { id = inventory.Id }, inventory);
		}

		// DELETE: api/inventory/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id:Guid}")]
		public async Task<IActionResult> DeleteInventory([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Inventory inventory = await _context.Inventory.FindAsync(id);
			if (inventory == null)
			{
				return HttpNotFound();
			}

			_context.Inventory.Remove(inventory);
			await _context.SaveChangesAsync();

			return Ok(inventory);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context.Dispose();
			}

			base.Dispose(disposing);
		}

		private bool InventoryExists(Guid id)
		{
			return _context.Inventory.Count(e => e.Id == id) > 0;
		}
	}
}
