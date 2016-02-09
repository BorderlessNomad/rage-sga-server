using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/inventory")]
    public class InventoryController : ApiController
    {
        public InventoryController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/inventory
        [HttpGet]
        public IEnumerable<Inventory> GetInventory()
        {
            return this._context.Inventory;
        }

        // GET: api/inventory/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id:Guid}", Name = "GetInventory")]
        public async Task<IActionResult> GetInventory([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var inventory = await this._context.Inventory.FindAsync(id);

            if (inventory == null)
            {
                return this.HttpNotFound();
            }

            return this.Ok(inventory);
        }

        // PUT: api/inventory/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> PutInventory([FromRoute] Guid id, [FromBody] Inventory inventory)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            if (id != inventory.Id)
            {
                return this.HttpBadRequest();
            }

            this._context.Entry(inventory).State = EntityState.Modified;

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!this.InventoryExists(id))
                {
                    return this.HttpNotFound();
                }
                throw;
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/inventory
        [HttpPost]
        public async Task<IActionResult> PostInventory([FromBody] Inventory inventory)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            this._context.Inventory.Add(inventory);
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.InventoryExists(inventory.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return this.CreatedAtRoute("GetInventory", new { id = inventory.Id }, inventory);
        }

        // DELETE: api/inventory/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteInventory([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var inventory = await this._context.Inventory.FindAsync(id);
            if (inventory == null)
            {
                return this.HttpNotFound();
            }

            this._context.Inventory.Remove(inventory);
            await this._context.SaveChangesAsync();

            return this.Ok(inventory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool InventoryExists(Guid id)
        {
            return this._context.Inventory.Count(e => e.Id == id) > 0;
        }
    }
}