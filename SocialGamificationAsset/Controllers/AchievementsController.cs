using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/achievements")]
    public class AchievementsController : ApiController
    {
        public AchievementsController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/achievements
        [HttpGet]
        public IEnumerable<Achievement> GetAchievements()
        {
            return this._context.Achievements;
        }

        // GET: api/achievements/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id:Guid}", Name = "GetAchievement")]
        public async Task<IActionResult> GetAchievement([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var achievement = await this._context.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return this.HttpNotFound();
            }

            return this.Ok(achievement);
        }

        // PUT: api/achievements/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> PutAchievement([FromRoute] Guid id, [FromBody] Achievement achievement)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            if (id != achievement.Id)
            {
                return this.HttpBadRequest();
            }

            this._context.Entry(achievement).State = EntityState.Modified;

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!this.AchievementExists(id))
                {
                    return this.HttpNotFound();
                }

                throw;
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/achievements
        [HttpPost]
        public async Task<IActionResult> PostAchievement([FromBody] Achievement achievement)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            this._context.Achievements.Add(achievement);
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.AchievementExists(achievement.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return this.CreatedAtRoute("GetAchievement", new { id = achievement.Id }, achievement);
        }

        // DELETE: api/achievements/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteAchievement([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var achievement = await this._context.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return this.HttpNotFound();
            }

            this._context.Achievements.Remove(achievement);
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.AchievementExists(achievement.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }

                throw;
            }

            return this.Ok(achievement);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool AchievementExists(Guid id)
        {
            return this._context.Achievements.Count(e => e.Id == id) > 0;
        }
    }
}