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
            return _context.Achievements;
        }

        // GET: api/achievements/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id:Guid}", Name = "GetAchievement")]
        public async Task<IActionResult> GetAchievement([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return HttpNotFound();
            }

            return Ok(achievement);
        }

        // PUT: api/achievements/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> PutAchievement([FromRoute] Guid id, [FromBody] Achievement achievement)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != achievement.Id)
            {
                return HttpBadRequest();
            }

            _context.Entry(achievement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!AchievementExists(id))
                {
                    return HttpNotFound();
                }

                throw;
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/achievements
        [HttpPost]
        public async Task<IActionResult> PostAchievement([FromBody] Achievement achievement)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.Achievements.Add(achievement);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AchievementExists(achievement.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return CreatedAtRoute("GetAchievement", new { id = achievement.Id }, achievement);
        }

        // DELETE: api/achievements/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteAchievement([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return HttpNotFound();
            }

            _context.Achievements.Remove(achievement);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AchievementExists(achievement.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }

                throw;
            }

            return Ok(achievement);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool AchievementExists(Guid id)
        {
            return _context.Achievements.Count(e => e.Id == id) > 0;
        }
    }
}