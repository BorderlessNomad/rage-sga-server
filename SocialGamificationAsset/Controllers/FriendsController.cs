using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGAControllers.Controllers
{
	[Produces("application/json")]
	[Route("api/friends")]
	public class FriendsController : Controller
	{
		private SocialGamificationAssetContext _context;

		public FriendsController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		// GET: api/friends
		[HttpGet]
		public IEnumerable<Friend> GetFriend()
		{
			return _context.Friends;
		}

		// GET: api/friends/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id:Guid}", Name = "GetFriend")]
		public async Task<IActionResult> GetFriend([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Friend friend = await _context.Friends.FindAsync(id);

			if (friend == null)
			{
				return HttpNotFound();
			}

			return Ok(friend);
		}

		// PUT: api/friends/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id:Guid}")]
		public async Task<IActionResult> PutFriend([FromRoute] Guid id, [FromBody] Friend friend)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (id != friend.Id)
			{
				return HttpBadRequest();
			}

			_context.Entry(friend).State = System.Data.Entity.EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FriendExists(id))
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

		// POST: api/friends
		[HttpPost]
		public async Task<IActionResult> PostFriend([FromBody] Friend friend)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.Friends.Add(friend);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (FriendExists(friend.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetFriend", new { id = friend.Id }, friend);
		}

		// DELETE: api/friends/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id:Guid}")]
		public async Task<IActionResult> DeleteFriend([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Friend friend = await _context.Friends.FindAsync(id);
			if (friend == null)
			{
				return HttpNotFound();
			}

			_context.Friends.Remove(friend);
			await _context.SaveChangesAsync();

			return Ok(friend);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool FriendExists(Guid id)
		{
			return _context.Friends.Count(e => e.Id == id) > 0;
		}
	}
}
