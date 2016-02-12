using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/targets")]
    public class TargetsController : ApiController
    {
        public TargetsController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/targets
        [HttpGet]
        public IEnumerable<Target> GetTarget()
        {
            return _context.Targets;
        }

        // GET: api/targets/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id}", Name = "GetTarget")]
        public async Task<IActionResult> GetTarget([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var test = await _context.Targets.FindAsync(id);

            if (test == null)
            {
                return HttpNotFound();
            }

            return Ok(test);
        }

        // PUT: api/targets/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarget([FromRoute] Guid id, [FromBody] Target test)
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

            return CreatedAtRoute("GetTarget", new { id = test.Id }, test);
        }

        // POST: api/targets
        [HttpPost]
        [ResponseType(typeof(Target))]
        public async Task<IActionResult> PostTarget([FromBody] Target test)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.Targets.Add(test);

            await SaveChangesAsync();

            return CreatedAtRoute("GetTarget", new { id = test.Id }, test);
        }

        // DELETE: api/targets/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarget([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var test = await _context.Targets.FindAsync(id);
            if (test == null)
            {
                return HttpNotFound();
            }

            _context.Targets.Remove(test);

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