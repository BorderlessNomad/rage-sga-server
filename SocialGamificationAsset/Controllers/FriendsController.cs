using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/friends")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class FriendsController : Controller
	{
		private SocialGamificationAssetContext _context;

		public FriendsController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		private Session _session;

		public Session session
		{
			get { return GetSession(); }
		}

		protected Session GetSession()
		{
			if (_session == null)
			{
				_session = HttpContext.Session.GetObjectFromJson<Session>("__session");
			}

			return _session;
		}

		// GET: api/friends/all
		[HttpGet]
		[Route("all", Name = "GetAllFriends")]
		public async Task<IActionResult> GetAllFriends()
		{
			if (session == null || session.Player == null)
			{
				return HttpNotFound("Invalid Session.");
			}

			IList<Actor> friends = await session.Player.Friends(_context).ToListAsync();

			return Ok(friends);
		}

		// GET: api/friends
		// GET: api/friends/accepted
		// GET: api/friends/pending
		// GET: api/friends/declined
		[HttpGet]
		[Route("{state?}", Name = "GetMyFriends")]
		public async Task<IActionResult> GetMyFriends([FromRoute] string state = "accepted")
		{
			if (session == null || session.Player == null)
			{
				return HttpNotFound("Invalid Session.");
			}

			FriendState friendshipStatus = FriendState.Accepted;
			if (state == "pending")
			{
				friendshipStatus = FriendState.Pending;
			}
			else if (state == "declined")
			{
				friendshipStatus = FriendState.Declined;
			}

			IList<Actor> friends = await session.Player.Friends(_context, friendshipStatus).ToListAsync();

			return Ok(friends);
		}

		// GET: api/friends/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet]
		[Route("{actorId:Guid}", Name = "GetActorFriends")]
		public async Task<IActionResult> GetActorFriends([FromRoute] Guid actorId)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(actorId)).FirstOrDefaultAsync();

			if (actor == null)
			{
				return HttpNotFound("No Actor Found.");
			}

			IList<Actor> friends = await actor.Friends(_context).ToListAsync();

			return Ok(friends);
		}

		// PUT: api/friends/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut]
		[Route("{friendId:Guid}")]
		public async Task<IActionResult> AddFriend([FromRoute] Guid friendId)
		{
			if (session == null || session.Player == null)
			{
				return HttpBadRequest("Error with your session.");
			}

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(friendId)).FirstOrDefaultAsync();

			if (actor == null)
			{
				return HttpNotFound("No such Actor found.");
			}

			Friend friend = await _context.Friends
				.Where(Friend.IsFriend(friendId, session.Player.Id))
				.FirstOrDefaultAsync();

			if (friend != null)
			{
				if (friend.State != FriendState.Accepted)
				{
					return HttpBadRequest("Friend Request already sent.");
				}

				return HttpBadRequest("You are aleady friend with this Actor.");
			}

			Friend newFriend = new Friend()
			{
				RequesterId = session.Player.Id,
				RequesteeId = friendId
			};

			_context.Friends.Add(newFriend);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException e)
			{
				throw e;
			}

			return Ok(newFriend);
		}

		// Delete: api/friends/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete]
		[Route("{friendId:Guid}")]
		public async Task<IActionResult> Unfriend([FromRoute] Guid friendId)
		{
			if (session == null || session.Player == null)
			{
				return HttpBadRequest("Error with your session.");
			}

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(friendId)).FirstOrDefaultAsync();

			if (actor == null)
			{
				return HttpNotFound("No such Actor found.");
			}

			Friend friend = await _context.Friends
				.Where(Friend.IsFriend(friendId, session.Player.Id))
				.FirstOrDefaultAsync();

			if (friend == null)
			{
				return HttpBadRequest("Friend is not in list.");
			}

			_context.Friends.Remove(friend);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException e)
			{
				throw e;
			}

			return Ok("Friend Removed fromt the list.");
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
