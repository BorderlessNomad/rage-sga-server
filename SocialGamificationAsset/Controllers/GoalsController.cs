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
        [HttpGet("", Name = "GetGoals")]
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

        // GET: api/goals/actor
        /// <summary>
        ///     Get <see cref="Player" /> 's Goals
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet("actor", Name = "GetPlayerGoals")]
        [ResponseType(typeof(IList<ActorGoal>))]
        public async Task<IActionResult> GetPlayerGoals()
        {
            IList<ActorGoal> actorGoals =
                await
                _context.ActorGoal.Where(g => g.ActorId.Equals(session.Player.Id)).Include(g => g.Goal).ToListAsync();

            return Ok(actorGoals);
        }

        // GET: api/goals/actor/936da01f-9abd-4d9d-80c7-02af85c822a8
        /// <summary>
        ///     Get <see cref="Actor" /> 's Goals
        /// </summary>
        /// <param name="id">GUID of <see cref="Player" /></param>
        /// <returns>
        /// </returns>
        [HttpGet("actor/{id:Guid}", Name = "GetActorGoals")]
        [ResponseType(typeof(IList<ActorGoal>))]
        public async Task<IActionResult> GetActorGoals([FromRoute] Guid id)
        {
            var actor = await _context.Actors.Where(p => p.Id.Equals(id)).FirstOrDefaultAsync();
            if (actor == null)
            {
                return Helper.HttpNotFound("No such Actor found.");
            }

            IList<ActorGoal> actorGoals =
                await _context.ActorGoal.Where(g => g.ActorId.Equals(actor.Id)).Include(g => g.Goal).ToListAsync();

            return Ok(actorGoals);
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

            return CreatedAtRoute("GetGoal", new { id = goal.Id }, goal);
        }

        // POST: api/goals
        [HttpPost]
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

            return CreatedAtRoute("GetGoal", new { id = goal.Id }, goal);
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