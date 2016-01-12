using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/achievements")]
	public class AchievementsController : Controller
	{
		private SocialGamificationAssetContext _context;

		public AchievementsController(SocialGamificationAssetContext context)
		{
			_context = context;
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

			Achievement achievement = await _context.Achievements.FindAsync(id);

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

			_context.Entry(achievement).State = System.Data.Entity.EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!AchievementExists(id))
				{
					return HttpNotFound();
				}
				else
				{
					throw;
				}
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
				else
				{
					throw;
				}
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

			Achievement achievement = await _context.Achievements.FindAsync(id);
			if (achievement == null)
			{
				return HttpNotFound();
			}

			_context.Achievements.Remove(achievement);
			await _context.SaveChangesAsync();

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
