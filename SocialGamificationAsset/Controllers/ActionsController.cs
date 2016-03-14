using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

using Action = SocialGamificationAsset.Models.Action;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/actions")]
    public class ActionsController : ApiController
    {
        public ActionsController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/actions
        [HttpGet]
        public IEnumerable<Action> GetAction()
        {
            return _context.Actions;
        }

        // GET: api/actions/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id}", Name = "GetAction")]
        public async Task<IActionResult> GetAction([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var action = await _context.Actions.FindAsync(id);

            if (action == null)
            {
                return Helper.HttpNotFound("No Action found.");
            }

            return Ok(action);
        }

        // PUT: api/actions/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAction([FromRoute] Guid id, [FromBody] Action action)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var actionMatch = await _context.Actions.Where(g => g.Id.Equals(id)).FirstOrDefaultAsync();
            if (actionMatch == null)
            {
                return Helper.HttpNotFound("No such Action found.");
            }


            _context.Entry(actionMatch).State = EntityState.Modified;

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return Ok(actionMatch);
        }

        // POST: api/actions/send
        [HttpPost("send")]
        [ResponseType(typeof(ActionForm))]
        public async Task<IActionResult> SendAction([FromBody] ActionForm action)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            foreach (Goal goal in await _context.Goals.Include(g => g.Actions).Include(g => g.Rewards.Select(r => r.AttributeType)).ToListAsync())
            {
                await goal.CalculateRewardFromAction(_context, action.Verb);
            }

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }
            return Ok();
        }

        // POST: api/actions
        [HttpPost]
        [ResponseType(typeof(Action))]
        public async Task<IActionResult> PostAction([FromBody] Action action)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var actTest = await _context.Activities.FindAsync(action.ActivityId);

            if (actTest == null)
            {
                return Helper.HttpNotFound("Invalid ActivityId.");
            }

            var goalTest = await _context.Goals.FindAsync(action.GoalId);

            if (goalTest == null)
            {
                return Helper.HttpNotFound("Invalid GoalId.");
            }
            _context.Actions.Add(action);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetAction", new { id = action.Id }, action);
        }

        // DELETE: api/actions/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAction([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var action = await _context.Actions.FindAsync(id);
            if (action == null)
            {
                return Helper.HttpNotFound("No Action found.");
            }

            _context.Actions.Remove(action);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return Ok(action);
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