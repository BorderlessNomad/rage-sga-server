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

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(session.Actor.Id)).Include(a => a.Friends).FirstAsync();

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

			Actor actor = await _context.Actors.Where(a => a.Id.Equals(actorId)).Include(a => a.Friends).FirstAsync();

			if (actor == null)
			{
				return HttpNotFound("No Actor Found.");
			}

			return Ok(actor.Friends);
		}

		// POST: api/friends/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPost]
		[Route("{id:Guid}")]
		public async Task<IActionResult> AddFriend([FromRoute] Guid id)
		{
			if (session == null || session.Actor == null)
			{
				return HttpBadRequest("Error with your session.");
			}

			Friend friend = await session.Actor.AddFriend(_context, id);

			if (friend != null)
			{
				return HttpBadRequest("Friend not in list");
			}

			return Ok(friend);
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

			Friend friend = await session.Actor.UnFriend(_context, id);

			if (friend != null)
			{
				return HttpBadRequest("Friend not in list");
			}

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
