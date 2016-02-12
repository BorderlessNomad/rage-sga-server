using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/rewards")]
    public class RewardsController : ApiController
    {
        public RewardsController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/rewards
        [HttpGet]
        public IEnumerable<Reward> GetReward()
        {
            return _context.Rewards;
        }

        // GET: api/rewards/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id}", Name = "GetReward")]
        public async Task<IActionResult> GetReward([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var test = await _context.Rewards.FindAsync(id);

            if (test == null)
            {
                return HttpNotFound();
            }

            return Ok(test);
        }

        // PUT: api/rewards/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReward([FromRoute] Guid id, [FromBody] Reward test)
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

            return CreatedAtRoute("GetReward", new { id = test.Id }, test);
        }

        // POST: api/rewards
        [HttpPost]
        [ResponseType(typeof(Reward))]
        public async Task<IActionResult> PostReward([FromBody] Reward test)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.Rewards.Add(test);

            await SaveChangesAsync();

            return CreatedAtRoute("GetReward", new { id = test.Id }, test);
        }

        // DELETE: api/rewards/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReward([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var test = await _context.Rewards.FindAsync(id);
            if (test == null)
            {
                return HttpNotFound();
            }

            _context.Rewards.Remove(test);

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