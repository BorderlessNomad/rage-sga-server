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

using Attribute = SocialGamificationAsset.Models.Attribute;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/attributes")]
    public class AttributesController : ApiController
    {
        public AttributesController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/attributes
        [HttpGet]
        public IEnumerable<Attribute> GetAttribute()
        {
            return _context.Attributes;
        }

        // GET: api/attributes/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id}", Name = "GetAttribute")]
        public async Task<IActionResult> GetAttribute([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var test = await _context.Attributes.FindAsync(id);

            if (test == null)
            {
                return HttpNotFound();
            }

            return Ok(test);
        }

        // PUT: api/attributes/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttribute([FromRoute] Guid id, [FromBody] Attribute test)
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
                if (!AttributeExists(id))
                {
                    return HttpNotFound();
                }
                throw;
            }

            return CreatedAtRoute("GetAttribute", new { id = test.Id }, test);
        }

        // POST: api/attributes
        [HttpPost]
        [ResponseType(typeof(Attribute))]
        public async Task<IActionResult> PostAttribute([FromBody] Attribute test)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.Attributes.Add(test);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AttributeExists(test.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return CreatedAtRoute("GetAttribute", new { id = test.Id }, test);
        }

        // DELETE: api/attributes/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttribute([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var test = await _context.Attributes.FindAsync(id);
            if (test == null)
            {
                return HttpNotFound();
            }

            _context.Attributes.Remove(test);
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

        private bool AttributeExists(Guid id)
        {
            return _context.Attributes.Count(e => e.Id == id) > 0;
        }
    }
}