using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/attributes/types")]
    public class AttributeTypesController : ApiController
    {
        public AttributeTypesController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/attributes/types
        [HttpGet]
        public IEnumerable<AttributeType> GetAttributeType()
        {
            return this._context.AttributeTypes;
        }

        // GET: api/attributes/types/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id}", Name = "GetAttributeType")]
        public async Task<IActionResult> GetAttributeType([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var test = await this._context.AttributeTypes.FindAsync(id);

            if (test == null)
            {
                return this.HttpNotFound();
            }

            return this.Ok(test);
        }

        // PUT: api/attributes/types/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttributeType([FromRoute] Guid id, [FromBody] AttributeType test)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            if (id != test.Id)
            {
                return this.HttpBadRequest();
            }

            this._context.Entry(test)
                .State = EntityState.Modified;

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!this.AttributeTypeExists(id))
                {
                    return this.HttpNotFound();
                }
                throw;
            }

            return this.CreatedAtRoute("GetAttributeType", new { id = test.Id }, test);
        }

        // POST: api/attributes/types
        [HttpPost]
        [ResponseType(typeof(AttributeType))]
        public async Task<IActionResult> PostAttributeType([FromBody] AttributeType test)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            this._context.AttributeTypes.Add(test);
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.AttributeTypeExists(test.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return this.CreatedAtRoute("GetAttributeType", new { id = test.Id }, test);
        }

        // DELETE: api/attributes/types/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttributeType([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var test = await this._context.AttributeTypes.FindAsync(id);
            if (test == null)
            {
                return this.HttpNotFound();
            }

            this._context.AttributeTypes.Remove(test);
            await this._context.SaveChangesAsync();

            return this.Ok(test);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool AttributeTypeExists(Guid id)
        {
            return this._context.AttributeTypes.Count(e => e.Id == id) > 0;
        }
    }
}