using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Http;
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
        public IEnumerable<Goal> GetGoal()
        {
            return this._context.Goals;
        }

        // GET: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id}", Name = "GetGoal")]
        public async Task<IActionResult> GetGoal([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var test = await this._context.Goals.FindAsync(id);

            if (test == null)
            {
                return this.HttpNotFound();
            }

            return this.Ok(test);
        }

        // PUT: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoal([FromRoute] Guid id, [FromBody] Goal test)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            if (id != test.Id)
            {
                return this.HttpBadRequest();
            }

            this._context.Entry(test)
                .State = EntityState.Modified;

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!this.GoalExists(id))
                {
                    return this.HttpNotFound();
                }
                throw;
            }

            return this.CreatedAtRoute("GetGoal", new { id = test.Id }, test);
        }

        // POST: api/goals
        [HttpPost]
        [ResponseType(typeof(Goal))]
        public async Task<IActionResult> PostGoal([FromBody] Goal test)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            this._context.Goals.Add(test);
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.GoalExists(test.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return this.CreatedAtRoute("GetGoal", new { id = test.Id }, test);
        }

        // DELETE: api/goals/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var test = await this._context.Goals.FindAsync(id);
            if (test == null)
            {
                return this.HttpNotFound();
            }

            this._context.Goals.Remove(test);
            await this._context.SaveChangesAsync();

            return this.Ok(test);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool GoalExists(Guid id)
        {
            return this._context.Goals.Count(e => e.Id == id) > 0;
        }
    }
}