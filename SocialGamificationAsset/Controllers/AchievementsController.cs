using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

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
        [ResponseType(typeof(IList<Achievement>))]
        public async Task<IActionResult> GetAchievements()
        {
            IList<Achievement> achievements =
                await _context.Achievements.Where(a => a.ActorId.Equals(session.Player.Id)).ToListAsync();

            return Ok(achievements);
        }

        // GET: api/achievements/936da01f-9abd-4d9d-80c7-02af85c822a8
        [ResponseType(typeof(Achievement))]
        [HttpGet("{id:Guid}", Name = "GetAchievement")]
        public async Task<IActionResult> GetAchievement([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return Helper.HttpNotFound("No Achievement found.");
            }

            return Ok(achievement);
        }

        // PUT: api/achievements/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id:Guid}")]
        [ResponseType(typeof(Achievement))]
        public async Task<IActionResult> PutAchievement([FromRoute] Guid id, [FromBody] Achievement achievement)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            if (id != achievement.Id)
            {
                return Helper.HttpBadRequest("Invalid Achievement Id.");
            }

            _context.Entry(achievement).State = EntityState.Modified;

            await SaveChangesAsync();

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/achievements
        [HttpPost]
        [ResponseType(typeof(Achievement))]
        public async Task<IActionResult> PostAchievement([FromBody] Achievement achievement)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            _context.Achievements.Add(achievement);

            await SaveChangesAsync();

            return CreatedAtRoute("GetAchievement", new { id = achievement.Id }, achievement);
        }

        // DELETE: api/achievements/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id:Guid}")]
        [ResponseType(typeof(Achievement))]
        public async Task<IActionResult> DeleteAchievement([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return Helper.HttpNotFound("No Achievement found.");
            }

            _context.Achievements.Remove(achievement);

            await SaveChangesAsync();

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
    }
}