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

            var test = await _context.AttributeTypes.FindAsync(id);

            if (test == null)
            {
                return HttpNotFound();
            }

            return Ok(test);
        }

        // PUT: api/attributes/types/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttributeType([FromRoute] Guid id, [FromBody] AttributeType test)
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

            await SaveChangesAsync();

            return CreatedAtRoute("GetAttributeType", new { id = test.Id }, test);
        }

        // POST: api/attributes/types
        [HttpPost]
        [ResponseType(typeof(AttributeType))]
        public async Task<IActionResult> PostAttributeType([FromBody] AttributeType test)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.AttributeTypes.Add(test);

            await SaveChangesAsync();

            return CreatedAtRoute("GetAttributeType", new { id = test.Id }, test);
        }

        // DELETE: api/attributes/types/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttributeType([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var test = await _context.AttributeTypes.FindAsync(id);
            if (test == null)
            {
                return HttpNotFound();
            }

            _context.AttributeTypes.Remove(test);

            await SaveChangesAsync();

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
    }
}