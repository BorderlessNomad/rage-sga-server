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
	[Route("api/players")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class PlayersController : Controller
	{
		private SocialGamificationAssetContext _context;

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

		public PlayersController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		// GET: api/players/whoami
		[HttpGet]
		[Route("whoami")]
		public async Task<IActionResult> WhoAmI()
		{
			if (session != null && session.Player != null)
			{
				return Ok(session.Player);
			}

			return HttpNotFound();
		}

		// GET: api/players
		[HttpGet]
		public async Task<IActionResult> GetAllPlayers()
		{
			IList<Player> players = await _context.Players.ToListAsync();

			if (players == null || players.Count < 1)
			{
				return HttpNotFound("No Player Found.");
			}

			return Ok(players);
		}

		// GET: api/players/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id:Guid}", Name = "GetPlayer")]
		public async Task<IActionResult> GetPlayer([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Player player = await _context.Players.FindAsync(id);

			if (player == null)
			{
				return HttpBadRequest("Invalid PlayerId");
			}

			return Ok(player);
		}

		// PUT: api/players/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id:Guid}")]
		public async Task<IActionResult> PutPlayer([FromRoute] Guid id, [FromBody] UserForm form)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Player player = await _context.Players.Where(p => p.Id.Equals(id)).FirstOrDefaultAsync();

			if (player == null)
			{
				return HttpNotFound("No Player Found.");
			}

			_context.Entry(player).State = EntityState.Modified;

			if (!String.IsNullOrWhiteSpace(form.Username) && player.Username != form.Username)
			{
				if (await Player.ExistsUsername(_context, form.Username))
				{
					return HttpBadRequest("Player with this Username already exists.");
				}

				player.Username = form.Username;
			}

			if (!String.IsNullOrWhiteSpace(form.Email) && player.Email != form.Email)
			{
				if (await Player.ExistsEmail(_context, form.Email))
				{
					return HttpBadRequest("Player with this Email already exists.");
				}

				player.Email = form.Email;
			}

			if (!String.IsNullOrWhiteSpace(form.Email))
			{
				player.Password = Helper.HashPassword(form.Password);
			}

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (!PlayerExists(id))
				{
					return HttpBadRequest("Invalid PlayerId");
				}
				else
				{
					throw;
				}
			}

			// Store the CustomData
			IList<CustomData> customData = CustomData.Parse(form.CustomData, player.Id, CustomDataType.Player);

			_context.CustomData.AddRange(customData);

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException e)
			{
				throw e;
			}

			return CreatedAtRoute("GetPlayer", new { id = player.Id }, player);
		}

		// POST: api/players
		[HttpPost]
		public async Task<IActionResult> PostPlayer([FromBody] Player player)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.Players.Add(player);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (PlayerExists(player.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetPlayer", new { id = player.Id }, player);
		}

		// DELETE: api/players/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id:Guid}")]
		public async Task<IActionResult> DeletePlayer([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Player player = await _context.Players.FindAsync(id);
			if (player == null)
			{
				return HttpBadRequest("Invalid PlayerId");
			}

			player.IsEnabled = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (!PlayerExists(id))
				{
					return HttpNotFound("No Player Found.");
				}
				else
				{
					throw;
				}
			}

			return Ok(player);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context.Dispose();
			}

			base.Dispose(disposing);
		}

		private bool PlayerExists(Guid id)
		{
			return _context.Players.Count(e => e.Id == id) > 0;
		}
	}
}
