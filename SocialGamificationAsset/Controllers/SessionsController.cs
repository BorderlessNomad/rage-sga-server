using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/sessions")]
	public class SessionsController : Controller
	{
		private SocialGamificationAssetContext _context;

		public SessionsController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		// GET: api/sessions/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id:Guid}", Name = "GetSession")]
		public async Task<IActionResult> GetSession([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Session session = await _context.Sessions.Include(s => s.Player).Where(s => s.Id.Equals(id)).FirstOrDefaultAsync();

			if (session == null)
			{
				return HttpNotFound();
			}

			return Ok(session);
		}

		// POST: api/sessions
		[HttpPost]
		public async Task<IActionResult> Login([FromBody] UserForm login)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (String.IsNullOrWhiteSpace(login.Username) && String.IsNullOrWhiteSpace(login.Email))
			{
				return HttpBadRequest("Either Username or Email is required.");
			}

			if (String.IsNullOrWhiteSpace(login.Password))
			{
				return HttpBadRequest("Password is required.");
			}

			IQueryable<Player> query = _context.Players;

			if (!String.IsNullOrWhiteSpace(login.Username))
			{
				query = query.Where(a => a.Username.Equals(login.Username));
			}

			if (!String.IsNullOrWhiteSpace(login.Email))
			{
				query = query.Where(a => a.Email.Equals(login.Email));
			}

			Player player = await query.FirstOrDefaultAsync();

			if (player == null)
			{
				return HttpNotFound("No such Player found.");
			}

			if (!Helper.ValidatePassword(login.Password, player.Password))
			{
				return new ContentResult()
				{
					StatusCode = StatusCodes.Status401Unauthorized,
					Content = "Invalid Login Details."
				};
			}

			Session session = new Session()
			{
				Player = player
			};

			_context.Sessions.Add(session);

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (SessionExists(session.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetSession", new { id = session.Id }, session);
		}

		// DELETE: api/sessions/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id:Guid}")]
		public async Task<IActionResult> Logout([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Session session = await _context.Sessions.FindAsync(id);
			if (session == null)
			{
				return HttpNotFound("No such Session found.");
			}

			session.IsExpired = true;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (SessionExists(session.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return Ok(session);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context.Dispose();
			}

			base.Dispose(disposing);
		}

		private bool SessionExists(Guid id)
		{
			return _context.Sessions.Count(e => e.Id == id) > 0;
		}
	}
}
