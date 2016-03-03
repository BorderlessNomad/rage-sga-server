using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/goals")]
    public class GoalsController : ApiController
    {
        public GoalsController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/goals
        [HttpGet]
        [ResponseType(typeof(IList<Goal>))]
        public async Task<IActionResult> GetGoals()
        {
            IList<Goal> goals =
                await
                _context.ActorGoal.Where(g => g.ActorId.Equals(session.Player.Id))
                        .Include(g => g.Goal)
                        .Select(g => g.Goal)
                        .ToListAsync();

            return Ok(goals);
        }

        // GET: api/goals/detailed
        [HttpGet("detailed", Name = "GetGoalsDetailed")]
        [ResponseType(typeof(IList<Goal>))]
        public async Task<IActionResult> GetGoalsDetailed()
        {
            IList<Goal> goals =
                await
                _context.ActorGoal.Where(g => g.ActorId.Equals(session.Player.Id))
                        .Include(g => g.Goal)
                        .Select(g => g.Goal)
                        .Include(g => g.Concern)
                        .Include(g => g.RewardResource)
                        .Include(g => g.Feedback)
                        .ToListAsync();

           foreach (Goal g in goals)
            {
                g.Activities = await _context.ActorGoal.Where(a => a.GoalId.Equals(g.Id)).Include(a => a.Activity).Select(a => a.Activity).ToListAsync();
                g.Roles = await _context.Roles.Where(r => r.GoalId.Equals(g.Id)).ToListAsync();
                g.Rewards = await _context.Rewards.Where(r => r.GoalId.Equals(g.Id)).ToListAsync();
                g.Targets = await _context.Targets.Where(t => t.GoalId.Equals(g.Id)).ToListAsync();
                g.Actions = await _context.Actions.Where(a => a.GoalId.Equals(g.Id)).ToListAsync();
            }

            return Ok(goals);
        }

        // GET: api/goals/{id}/activity
        [HttpGet("{id}/activity", Name = "GetGoalsByActivity")]
        [ResponseType(typeof(IList<Goal>))]
        public async Task<IActionResult> GetGoalsByActivity([FromRoute] Guid id)
        {
            IList<Goal> goals =
                await
                _context.Roles.Where(g => g.ActivityId.Equals(id))
                        .Include(g => g.Goal)
                        .Select(g => g.Goal)
                        .Include(g => g.Concern)
                        .Include(g => g.RewardResource)
                        .Include(g => g.Feedback)
                        .ToListAsync();

            foreach (Goal g in goals)
            {
                g.Activities = await _context.ActorGoal.Where(a => a.GoalId.Equals(g.Id)).Include(a => a.Activity).Select(a => a.Activity).ToListAsync();
                g.Roles = await _context.Roles.Where(r => r.GoalId.Equals(g.Id)).ToListAsync();
                g.Rewards = await _context.Rewards.Where(r => r.GoalId.Equals(g.Id)).ToListAsync();
                g.Targets = await _context.Targets.Where(t => t.GoalId.Equals(g.Id)).ToListAsync();
                g.Actions = await _context.Actions.Where(a => a.GoalId.Equals(g.Id)).ToListAsync();
            }

            return Ok(goals);
        }

        // GET: api/goals/{id}/actor
        [HttpGet("{id}/actor", Name = "GetActorGoal")]
        [ResponseType(typeof(ActorGoal))]
        public async Task<IActionResult> GetActorGoal([FromRoute] Guid id)
        {
            ActorGoal goal =
                await
                _context.ActorGoal.Where(g => g.GoalId.Equals(id)).Where(g => g.ActorId.Equals(session.Player.Id))
                        .Include(g => g.Actor)
                        .Include(g => g.Goal)
                        .Include(g => g.ConcernOutcome)
                        .Include(g => g.RewardResourceOutcome)
                        .Include(g => g.Activity)
                        .Include(g => g.Role)
                        .FirstOrDefaultAsync();

            return Ok(goal);
        }

        // GET: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id}", Name = "GetGoal")]
        [ResponseType(typeof(Goal))]
        public async Task<IActionResult> GetGoal([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
            {
                return Helper.HttpNotFound("No Goal found.");
            }

            return Ok(goal);
        }

        // GET: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id}/detailed", Name = "GetGoalDetailed")]
        [ResponseType(typeof(Goal))]
        public async Task<IActionResult> GetGoalDetailed([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
            {
                return Helper.HttpNotFound("No Goal found.");
            }
            goal.Concern = await _context.ConcernMatrix.FindAsync(goal.ConcernId);
            goal.RewardResource = await _context.RewardResourceMatrix.FindAsync(goal.RewardResourceId);
            goal.Feedback = await _context.GoalFeedback.FindAsync(goal.FeedbackId);
            goal.Activities = await _context.ActorGoal.Where(a => a.GoalId.Equals(goal.Id)).Include(a => a.Activity).Select(a => a.Activity).ToListAsync();
            goal.Roles = await _context.Roles.Where(r => r.GoalId.Equals(goal.Id)).ToListAsync();
            goal.Rewards = await _context.Rewards.Where(r => r.GoalId.Equals(goal.Id)).ToListAsync();
            goal.Targets = await _context.Targets.Where(t => t.GoalId.Equals(goal.Id)).ToListAsync();
            goal.Actions = await _context.Actions.Where(a => a.GoalId.Equals(goal.Id)).ToListAsync();

            return Ok(goal);
        }

        // PUT: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        [ResponseType(typeof(Goal))]
        public async Task<IActionResult> PutGoal([FromRoute] Guid id, [FromBody] Goal goal)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            if (id != goal.Id)
            {
                return Helper.HttpBadRequest("Invalid Goal Id.");
            }

            _context.Entry(goal).State = EntityState.Modified;

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetGoalDetailed", new { id = goal.Id }, goal);
        }

        // POST: api/goals
        [HttpPost("", Name = "PostGoal")]
        [ResponseType(typeof(Goal))]
        public async Task<IActionResult> PostGoal([FromBody] Goal goal)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            _context.Goals.Add(goal);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }
            return CreatedAtRoute("GetGoalDetailed", new { id = goal.Id }, goal);
        }

        // POST: api/goals/actors
        [HttpPost("actors", Name = "PostActorGoal")]
        [ResponseType(typeof(ActorGoal))]
        public async Task<IActionResult> PostActorGoal([FromBody] ActorGoal goal)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            _context.ActorGoal.Add(goal);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }
            return CreatedAtRoute("GetActorGoal", new { id = goal.GoalId }, goal);
        }

        // DELETE: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        [ResponseType(typeof(Goal))]
        public async Task<IActionResult> DeleteGoal([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
            {
                return Helper.HttpNotFound("No Goal found.");
            }

            _context.Goals.Remove(goal);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return Ok(goal);
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