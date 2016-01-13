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

			Session session = await _context.Sessions.FindAsync(id);

			if (session == null)
			{
				return HttpNotFound();
			}

			return Ok(session);
		}

		// PUT: api/sessions
		[HttpPut]
		public async Task<IActionResult> AddPlayer([FromBody] UserForm register)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (String.IsNullOrWhiteSpace(register.Username) && String.IsNullOrWhiteSpace(register.Email))
			{
				return HttpBadRequest("Either Username or Email is required.");
			}

			Player player = new Player();

			if (!String.IsNullOrWhiteSpace(register.Username))
			{
				if (await Player.ExistsUsername(_context, register.Username))
				{
					return HttpBadRequest("Username already exists.");
				}

				player.Username = register.Username;
			}

			if (!String.IsNullOrWhiteSpace(register.Email))
			{
				if (await Player.ExistsEmail(_context, register.Email))
				{
					return HttpBadRequest("Email already exists.");
				}

				player.Email = register.Email;
			}

			player.Password = Helper.HashPassword(register.Password);

			Session session = new Session
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

			Player player = new Player();

			if (!String.IsNullOrWhiteSpace(login.Username) && String.IsNullOrWhiteSpace(login.Email))
			{
				player = await _context.Players
					.Where(a => a.Username.Equals(login.Username))
					.FirstOrDefaultAsync();
			}
			else if (String.IsNullOrWhiteSpace(login.Username) && !String.IsNullOrWhiteSpace(login.Email))
			{
				player = await _context.Players
					.Where(a => a.Email.Equals(login.Email))
					.FirstOrDefaultAsync();
			}
			else if (!String.IsNullOrWhiteSpace(login.Username) && !String.IsNullOrWhiteSpace(login.Email))
			{
				player = await _context.Players
					.Where(a => a.Username.Equals(login.Username))
					.Where(a => a.Email.Equals(login.Email))
					.FirstOrDefaultAsync();
			}

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
