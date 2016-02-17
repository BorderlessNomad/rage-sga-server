using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;

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
            return _context.AttributeTypes;
        }

        // GET: api/attributes/types/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id}", Name = "GetAttributeType")]
        public async Task<IActionResult> GetAttributeType([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var type = await _context.AttributeTypes.FindAsync(id);

            if (type == null)
            {
                return HttpNotFound();
            }

            return Ok(type);
        }

        // PUT: api/attributes/types/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttributeType([FromRoute] Guid id, [FromBody] AttributeType type)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != type.Id)
            {
                return HttpBadRequest();
            }

            _context.Entry(type).State = EntityState.Modified;

            await SaveChangesAsync();

            return CreatedAtRoute("GetAttributeType", new { id = type.Id }, type);
        }

        // POST: api/attributes/types
        [HttpPost]
        [ResponseType(typeof(AttributeType))]
        public async Task<IActionResult> PostAttributeType([FromBody] AttributeType type)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.AttributeTypes.Add(type);

            await SaveChangesAsync();

            return CreatedAtRoute("GetAttributeType", new { id = type.Id }, type);
        }

        // DELETE: api/attributes/types/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttributeType([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var type = await _context.AttributeTypes.FindAsync(id);
            if (type == null)
            {
                return HttpNotFound();
            }

            _context.AttributeTypes.Remove(type);

            await SaveChangesAsync();

            return Ok(type);
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