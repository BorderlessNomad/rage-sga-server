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

        // GET: api/actions/936da01f-9abd-4d9d-80c7-02af85c822a8/relations
        [HttpGet("{id}/relations", Name = "GetActionRelation")]
        public async Task<IActionResult> GetActionRelation([FromRoute] Guid id)
        {
          if (!ModelState.IsValid)
          {
            return Helper.HttpBadRequest(ModelState);
          }

          var actionRelation = await _context.ActionRelations.FindAsync(id);

          if (actionRelation == null)
          {
            return Helper.HttpNotFound("No ActionRelation found.");
          }

          return Ok(actionRelation);
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

            Action actionMatch = await _context.Actions.Where(a => a.Verb.Equals(action.Verb)).FirstOrDefaultAsync();

            if (actionMatch == null)
            {
              return Helper.HttpNotFound("Invalid action verb.");
            }

            Reward reward = null;

            foreach (Goal goal in await _context.Goals.Include(g => g.Actions).Include(g => g.Rewards.Select(r => r.AttributeType)).ToListAsync())
            {
                reward = await goal.CalculateRewardFromAction(_context, action.Verb);
                if (reward != null)
                {
                    break;
                }
            }

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }
            return Ok(reward);
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
            if (action.ActivityId != Guid.Empty)
            {
              var actTest = await _context.Activities.FindAsync(action.ActivityId);

              if (actTest == null)
              {
                return Helper.HttpNotFound("Invalid ActivityId.");
              }
            }
            else if (action.Activity == null)
            {
              return Helper.HttpNotFound("No Activity found");
            }
            if (action.GoalId != Guid.Empty)
            {
              var goalTest = await _context.Goals.FindAsync(action.GoalId);

              if (goalTest == null)
              {
                return Helper.HttpNotFound("Invalid GoalId.");
              }
            }
            else if (action.Goal == null)
            {
              return Helper.HttpNotFound("No Goal found");
            }

            _context.Actions.Add(action);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetAction", new { id = action.Id }, action);
        }

        // POST: api/actions/relations
        [HttpPost("relations", Name = "PostActionRelation")]
        [ResponseType(typeof(ActionRelation))]
        public async Task<IActionResult> PostActionRelation([FromBody] ActionRelation actionRelation)
        {
          if (!ModelState.IsValid)
          {
            return Helper.HttpBadRequest(ModelState);
          }
          if (actionRelation.ActionId != Guid.Empty)
          {
            var actTest = await _context.Actions.FindAsync(actionRelation.ActionId);

            if (actTest == null)
            {
              return Helper.HttpNotFound("Invalid ActionId.");
            }
            actionRelation.Action = actTest;
          }
          else if (actionRelation.Action == null)
          {
            return Helper.HttpNotFound("No Action found");
          }

          _context.ActionRelations.Add(actionRelation);

          var error = await SaveChangesAsync();
          if (error != null)
          {
            return error;
          }

          return CreatedAtRoute("GetActionRelation", new { id = actionRelation.Id }, actionRelation);
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