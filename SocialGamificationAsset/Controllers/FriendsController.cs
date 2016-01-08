using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;
using System;
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

		// GET: api/friends
		[HttpGet]
		[Route("", Name = "GetMyFriends")]
		public async Task<IActionResult> GetMyFriends()
		{
			if (session == null || session.Actor == null)
			{
				return HttpNotFound("Invalid Session.");
			}

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(session.Actor.Id)).Include(a => a.Friends).FirstOrDefaultAsync();

			return Ok(actor.Friends);
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

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(actorId)).Include(a => a.Friends).FirstOrDefaultAsync();

			if (actor == null)
			{
				return HttpNotFound("No Actor Found.");
			}

			return Ok(actor.Friends);
		}

		// PUT: api/friends/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut]
		[Route("{id:Guid}")]
		public async Task<IActionResult> AddFriend([FromRoute] Guid id)
		{
			if (session == null || session.Actor == null)
			{
				return HttpBadRequest("Error with your session.");
			}

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(id)).Include(a => a.Friends).FirstOrDefaultAsync();

			if (actor == null)
			{
				return HttpNotFound("No such Actor found.");
			}

			Friend friend = await _context.Friends
				.Where(f =>
					(f.RequesterId.Equals(id) && f.RequesteeId.Equals(session.Actor.Id)) ||
					(f.RequesteeId.Equals(id) && f.RequesterId.Equals(session.Actor.Id))
				)
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
				RequesterId = session.Actor.Id,
				RequesteeId = id
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
		[Route("{id:Guid}")]
		public async Task<IActionResult> Unfriend([FromRoute] Guid id)
		{
			if (session == null || session.Actor == null)
			{
				return HttpBadRequest("Error with your session.");
			}

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(id)).Include(a => a.Friends).FirstOrDefaultAsync();

			if (actor == null)
			{
				return HttpNotFound("No such Actor found.");
			}

			Friend friend = await _context.Friends
				.Where(f =>
					(f.RequesterId.Equals(id) && f.RequesteeId.Equals(session.Actor.Id)) ||
					(f.RequesteeId.Equals(id) && f.RequesterId.Equals(session.Actor.Id))
				)
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
