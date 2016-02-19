using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;

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
                return Helper.HttpBadRequest(ModelState);
            }

            var attribute = await _context.Attributes.FindAsync(id);

            if (attribute == null)
            {
                return Helper.HttpNotFound("No Attribute found.");
            }

            return Ok(attribute);
        }

        // PUT: api/attributes/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttribute([FromRoute] Guid id, [FromBody] Attribute attribute)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            if (id != attribute.Id)
            {
                return Helper.HttpBadRequest("Invalid Attribute Id.");
            }

            _context.Entry(attribute).State = EntityState.Modified;

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetAttribute", new { id = attribute.Id }, attribute);
        }

        // POST: api/attributes
        [HttpPost]
        [ResponseType(typeof(Attribute))]
        public async Task<IActionResult> PostAttribute([FromBody] Attribute attribute)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            _context.Attributes.Add(attribute);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetAttribute", new { id = attribute.Id }, attribute);
        }

        // DELETE: api/attributes/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttribute([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var attribute = await _context.Attributes.FindAsync(id);
            if (attribute == null)
            {
                return Helper.HttpNotFound("No Attribute found.");
            }

            _context.Attributes.Remove(attribute);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return Ok(attribute);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}