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
    /// <param name="id">GUID of <see cref="Player" /></param>
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

    // GET: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8/role
    [HttpGet("{name}/role", Name = "GetGoalsByRole")]
    [ResponseType(typeof(IList<Goal>))]
    public async Task<IActionResult> GetGoalsByRole([FromRoute] string name)
    {

      if (string.IsNullOrEmpty(name))
      {
        return Helper.HttpNotFound("No Role found");
      }
      var roletest = await _context.Roles.Where(g => g.Name.Equals(name)).FirstOrDefaultAsync();
      if (roletest == null)
      {
        return Helper.HttpNotFound("No Role found for the passed name");
      }

      IList<Goal> goals =
          await
          _context.Roles.Where(g => g.Name.Equals(name))
                  .Include(g => g.Goal)
                  .Select(g => g.Goal)
                  .Include(g => g.Concern)
                  .Include(g => g.RewardResource)
                  .Include(g => g.Feedback)
                  .ToListAsync();

      foreach (Goal g in goals)
      {
        g.Roles = await _context.Roles.Where(r => r.GoalId.Equals(g.Id)).ToListAsync();
        g.Rewards = await _context.Rewards.Where(r => r.GoalId.Equals(g.Id)).ToListAsync();
        g.Targets = await _context.Targets.Where(t => t.GoalId.Equals(g.Id)).ToListAsync();
        g.Actions = await _context.Actions.Where(a => a.GoalId.Equals(g.Id)).ToListAsync();
        IList<Guid> activitiesIDs = g.Roles.AsEnumerable().Select(r => r.ActivityId).ToList();
        g.Activities = await _context.Activities.Where(a => activitiesIDs.Contains(a.Id)).OrderBy(a => a.Name).ToListAsync();
      }

      if (goals.Count == 0)
      {
        return Helper.HttpNotFound("No Goals found.");
      }

      return Ok(goals);
    }

    // GET: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8/actor
    [HttpGet("{id:Guid}/actor", Name = "GetActorGoal")]
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

      if (!ModelState.IsValid)
      {
        return Helper.HttpBadRequest(ModelState);
      }

      if (goal == null)
      {
        return Helper.HttpNotFound("No ActorGoal found.");
      }

      return Ok(goal);
    }

    // GET: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
    [HttpGet("{id:Guid}", Name = "GetGoal")]
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
    [HttpGet("{id:Guid}/detailed", Name = "GetGoalDetailed")]
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
      goal.Roles = await _context.Roles.Where(r => r.GoalId.Equals(goal.Id)).ToListAsync();
      IList<Guid> activitiesIDs = goal.Roles.AsEnumerable().Select(r => r.ActivityId).ToList();
      goal.Activities = await _context.Activities.Where(a => activitiesIDs.Contains(a.Id)).OrderBy(a => a.Name).ToListAsync();
      goal.Rewards = await _context.Rewards.Where(r => r.GoalId.Equals(goal.Id)).ToListAsync();
      goal.Targets = await _context.Targets.Where(t => t.GoalId.Equals(goal.Id)).ToListAsync();
      goal.Actions = await _context.Actions.Where(a => a.GoalId.Equals(goal.Id)).ToListAsync();

      return Ok(goal);
    }

    // PUT: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
    [HttpPut("{id:Guid}")]
    [ResponseType(typeof(Goal))]
    public async Task<IActionResult> PutGoal([FromRoute] Guid id, [FromBody] Goal form)
    {
      if (!ModelState.IsValid)
      {
        return Helper.HttpBadRequest(ModelState);
      }

      var goal = await _context.Goals.Where(g => g.Id.Equals(id)).FirstOrDefaultAsync();
      if (goal == null)
      {
        return Helper.HttpNotFound("No such Goal found.");
      }

      _context.Entry(goal).State = EntityState.Modified;
      goal.Description = form.Description;

      var error = await SaveChangesAsync();
      if (error != null)
      {
        return error;
      }

      return Ok(goal);
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
      if (goal.ConcernId != Guid.Empty)
      {
        var concerntest = await _context.ConcernMatrix.FindAsync(goal.ConcernId);
        if (concerntest == null)
        {
          return Helper.HttpNotFound("No Concern found for the passed ID");
        }
      }
      else if (goal.Concern == null)
      {
        return Helper.HttpNotFound("No Concern found");
      }
      if (goal.RewardResourceId != Guid.Empty)
      {
        var rrtest = await _context.RewardResourceMatrix.FindAsync(goal.RewardResourceId);
        if (rrtest == null)
        {
          return Helper.HttpNotFound("No RewardResource found for the passed ID");
        }
      }
      else if (goal.RewardResource == null)
      {
        return Helper.HttpNotFound("No RewardResource found");
      }
      if (goal.FeedbackId != Guid.Empty)
      {
        var fbtest = await _context.GoalFeedback.FindAsync(goal.FeedbackId);
        if (fbtest == null)
        {
          return Helper.HttpNotFound("No GoalFeedback found for the passed ID");
        }
      }
      else if (goal.Feedback == null)
      {
        return Helper.HttpNotFound("No GoalFeedback found");
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
      if (goal.ActivityId == Guid.Empty)
      {
        return Helper.HttpNotFound("No Activity found");
      }
      var activitytest = await _context.Activities.FindAsync(goal.ActivityId);
      if (activitytest == null)
      {
        return Helper.HttpNotFound("No Activity found for the passed ID");
      }
      if (goal.ActorId == Guid.Empty)
      {
        return Helper.HttpNotFound("No Actor found");
      }
      var actortest = await _context.Players.FindAsync(goal.ActorId);
      if (actortest == null)
      {
        return Helper.HttpNotFound("No Actor found for the passed ID");
      }
      if (goal.ConcernOutcomeId == Guid.Empty)
      {
        return Helper.HttpNotFound("No ConcernOutcome found");
      }
      var concerntest = await _context.ConcernMatrix.FindAsync(goal.ConcernOutcomeId);
      if (concerntest == null)
      {
        return Helper.HttpNotFound("No ConcernOutcome found for the passed ID");
      }
      if (goal.GoalId == Guid.Empty)
      {
        return Helper.HttpNotFound("No Goal found");
      }
      var goaltest = await _context.Goals.FindAsync(goal.GoalId);
      if (goaltest == null)
      {
        return Helper.HttpNotFound("No Goal found for the passed ID");
      }
      if (goal.RewardResourceOutcomeId == Guid.Empty)
      {
        return Helper.HttpNotFound("No RewardResourceOutcome found");
      }
      var rrtest = await _context.RewardResourceMatrix.FindAsync(goal.RewardResourceOutcomeId);
      if (rrtest == null)
      {
        return Helper.HttpNotFound("No RewardResourceOutcome found for the passed ID");
      }
      if (goal.RoleId == Guid.Empty)
      {
        return Helper.HttpNotFound("No Role found");
      }
      var roletest = await _context.Roles.FindAsync(goal.RoleId);
      if (roletest == null)
      {
        return Helper.HttpNotFound("No Role found for the passed ID");
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
    [HttpDelete("{id:Guid}")]
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

      goal.IsDeleted = true;

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