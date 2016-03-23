using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Helpers;
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
                return HttpResponseHelper.BadRequest(ModelState);
            }

            var reward = await _context.Rewards.FindAsync(id);

            if (reward == null)
            {
                return HttpResponseHelper.NotFound("No Reward found.");
            }

            return Ok(reward);
        }

        // PUT: api/rewards/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReward([FromRoute] Guid id, [FromBody] Reward reward)
        {
            if (!ModelState.IsValid)
            {
                return HttpResponseHelper.BadRequest(ModelState);
            }

            if (id != reward.Id)
            {
                return HttpResponseHelper.BadRequest("Invalid Reward Id.");
            }

            _context.Entry(reward).State = EntityState.Modified;

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetReward", new { id = reward.Id }, reward);
        }

        // POST: api/rewards
        [HttpPost]
        [ResponseType(typeof(Reward))]
        public async Task<IActionResult> PostReward([FromBody] Reward reward)
        {
            if (!ModelState.IsValid)
            {
                return HttpResponseHelper.BadRequest(ModelState);
            }

            _context.Rewards.Add(reward);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetReward", new { id = reward.Id }, reward);
        }

        // DELETE: api/rewards/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReward([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpResponseHelper.BadRequest(ModelState);
            }

            var reward = await _context.Rewards.FindAsync(id);
            if (reward == null)
            {
                return HttpResponseHelper.NotFound("No Reward found.");
            }

            _context.Rewards.Remove(reward);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return Ok(reward);
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