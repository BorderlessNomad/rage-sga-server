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
                return Helper.HttpBadRequest(ModelState);
            }

            var target = await _context.Targets.FindAsync(id);

            if (target == null)
            {
                return Helper.HttpNotFound("No Target found.");
            }

            return Ok(target);
        }

        // PUT: api/targets/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarget([FromRoute] Guid id, [FromBody] Target target)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            if (id != target.Id)
            {
                return Helper.HttpBadRequest("Invalid Target Id.");
            }

            _context.Entry(target).State = EntityState.Modified;

            await SaveChangesAsync();

            return CreatedAtRoute("GetTarget", new { id = target.Id }, target);
        }

        // POST: api/targets
        [HttpPost]
        [ResponseType(typeof(Target))]
        public async Task<IActionResult> PostTarget([FromBody] Target target)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            _context.Targets.Add(target);

            await SaveChangesAsync();

            return CreatedAtRoute("GetTarget", new { id = target.Id }, target);
        }

        // DELETE: api/targets/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarget([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var target = await _context.Targets.FindAsync(id);
            if (target == null)
            {
                return Helper.HttpNotFound("No Target found.");
            }

            _context.Targets.Remove(target);

            await SaveChangesAsync();

            return Ok(target);
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