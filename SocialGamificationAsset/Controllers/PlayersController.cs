using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace SocialGamificationAsset.Controllers
{
	[Route("api/players")]
	public class PlayersController : ApiController
	{
		/// <summary>
		///     <see cref="Player" /> API
		/// </summary>
		/// <param name="context"></param>
		public PlayersController(SocialGamificationAssetContext context)
			: base(context)
		{
		}

		// GET: api/players
		[HttpGet]
		public IEnumerable<Player> GetPlayer()
		{
			return _context.Players;
		}

		// GET: api/players/whoami
		/// <summary>
		///     Get <see cref="Player" /> Details
		/// </summary>
		/// <returns>
		/// </returns>
		[HttpGet("")]
		[HttpGet("whoami", Name = "WhoAmI")]
		[ResponseType(typeof(Player))]
		public IActionResult WhoAmI() => Ok(session.Player);

		// GET: api/players/936da01f-9abd-4d9d-80c7-02af85c822a8
		/// <summary>
		///     Get <see cref="Player" /> Details
		/// </summary>
		/// <param name="id">GUID of <see cref="Player" /></param>
		/// <returns>
		/// </returns>
		[HttpGet("{id:Guid}", Name = "GetPlayer")]
		[ResponseType(typeof(Player))]
		public async Task<IActionResult> GetPlayer([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return Helper.HttpBadRequest(ModelState);
			}

			var player = await _context.Players.FindAsync(id);
			if (player == null)
			{
				return Helper.HttpNotFound("No such Player found.");
			}

			return Ok(player);
		}

		// PUT: api/players/936da01f-9abd-4d9d-80c7-02af85c822a8
		/// <summary>
		///     Update <see cref="Player" /> 's information
		/// </summary>
		/// <param name="id">GUID of <see cref="Player" /></param>
		/// <param name="form">Infomation to be updated</param>
		/// <returns>
		/// </returns>
		[HttpPut("{id:Guid}", Name = "UpdatePlayer")]
		[ResponseType(typeof(Player))]
		public async Task<IActionResult> UpdatePlayer([FromRoute] Guid id, [FromBody] UserForm form)
		{
			if (!ModelState.IsValid)
			{
				return Helper.HttpBadRequest(ModelState);
			}

			var player = await _context.Players.FindAsync(id);

			if (player == null)
			{
				return Helper.HttpNotFound("No Player found.");
			}

			_context.Entry(player).State = EntityState.Modified;

			if (!string.IsNullOrWhiteSpace(form.Username) && player.Username != form.Username)
			{
				if (await Player.ExistsUsername(_context, form.Username))
				{
					return Helper.HttpBadRequest("Player with this Username already exists.");
				}

				player.Username = form.Username;
			}

			if (!string.IsNullOrWhiteSpace(form.Email) && player.Email != form.Email)
			{
				if (await Player.ExistsEmail(_context, form.Email))
				{
					return Helper.HttpBadRequest("Player with this Email already exists.");
				}

				player.Email = form.Email;
			}

			if (!string.IsNullOrWhiteSpace(form.Password))
			{
				player.Password = Helper.HashPassword(form.Password);
			}

			var error = await SaveChangesAsync();
			if (error != null)
			{
				return error;
			}

			// Add or Update the CustomData
			error = await player.AddOrUpdateCustomData(_context, form.CustomData);
			if (error != null)
			{
				return error;
			}

			return Ok(player);
		}

		// POST: api/players
		/// <summary>
		///     Add new <see cref="Player" />
		/// </summary>
		/// <param name="register"><see cref="UserForm" /> details.</param>
		/// <returns>
		/// </returns>
		[HttpPost("", Name = "AddPlayer")]
		[AllowAnonymous]
		[ResponseType(typeof(Player))]
		public async Task<IActionResult> AddPlayer([FromBody] UserForm register)
		{
			if (!ModelState.IsValid)
			{
				return Helper.HttpBadRequest(ModelState);
			}

			if (string.IsNullOrWhiteSpace(register.Username) && string.IsNullOrWhiteSpace(register.Email))
			{
				return Helper.HttpBadRequest("Either Username or Email is required.");
			}

			if (string.IsNullOrWhiteSpace(register.Password))
			{
				return Helper.HttpBadRequest("Password is required.");
			}

			var player = new Player();

			if (!string.IsNullOrWhiteSpace(register.Username))
			{
				if (await Player.ExistsUsername(_context, register.Username))
				{
					return Helper.HttpBadRequest("Player with this Username already exists.");
				}

				player.Username = register.Username;
			}

			if (!string.IsNullOrWhiteSpace(register.Email))
			{
				if (await Player.ExistsEmail(_context, register.Email))
				{
					return Helper.HttpBadRequest("Player with this Email already exists.");
				}

				player.Email = register.Email;
			}

			player.Password = Helper.HashPassword(register.Password);

			_context.Players.Add(player);

			var error = await SaveChangesAsync();
			if (error != null)
			{
				return error;
			}

			// Add or Update the CustomData
			error = await player.AddOrUpdateCustomData(_context, register.CustomData);
			if (error != null)
			{
				return error;
			}

			return CreatedAtRoute("GetPlayer", new { id = player.Id }, player);
		}

		// DELETE: api/players/936da01f-9abd-4d9d-80c7-02af85c822a8
		/// <summary>
		///     Delete a <see cref="Player" />
		/// </summary>
		/// <param name="id">GUID of <see cref="Player" /></param>
		/// <returns>
		/// </returns>
		[HttpDelete("{id:Guid}")]
		[ResponseType(typeof(Player))]
		public async Task<IActionResult> DeletePlayer([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return Helper.HttpBadRequest(ModelState);
			}

			var player = await _context.Players.FindAsync(id);
			if (player == null)
			{
				return Helper.HttpNotFound("No Player found.");
			}

			player.IsEnabled = false;

			var error = await SaveChangesAsync();
			if (error != null)
			{
				return error;
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
	}
}
