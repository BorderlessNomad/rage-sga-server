using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/players")]
    public class PlayersController : ApiController
    {
        public PlayersController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/players/whoami
        [HttpGet]
        [Route("whoami")]
        public async Task<IActionResult> WhoAmI()
        {
            if (this.session != null && this.session.Player != null)
            {
                return this.Ok(this.session.Player);
            }

            return this.HttpNotFound();
        }

        // GET: api/players
        [HttpGet]
        public async Task<IActionResult> GetAllPlayers()
        {
            IList<Player> players = await this._context.Players.ToListAsync();

            if (players == null || players.Count < 1)
            {
                return this.HttpNotFound("No Player Found.");
            }

            return this.Ok(players);
        }

        // GET: api/players/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id:Guid}", Name = "GetPlayer")]
        public async Task<IActionResult> GetPlayer([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var player = await this._context.Players.FindAsync(id);

            if (player == null)
            {
                return this.HttpBadRequest("Invalid PlayerId");
            }

            return this.Ok(player);
        }

        // PUT: api/players/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdatePlayer([FromRoute] Guid id, [FromBody] UserForm form)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var player = await this._context.Players.Where(p => p.Id.Equals(id))
                                   .FirstOrDefaultAsync();

            if (player == null)
            {
                return this.HttpNotFound("No Player Found.");
            }

            this._context.Entry(player)
                .State = EntityState.Modified;

            if (!string.IsNullOrWhiteSpace(form.Username) && player.Username != form.Username)
            {
                if (await Player.ExistsUsername(this._context, form.Username))
                {
                    return this.HttpBadRequest("Player with this Username already exists.");
                }

                player.Username = form.Username;
            }

            if (!string.IsNullOrWhiteSpace(form.Email) && player.Email != form.Email)
            {
                if (await Player.ExistsEmail(this._context, form.Email))
                {
                    return this.HttpBadRequest("Player with this Email already exists.");
                }

                player.Email = form.Email;
            }

            if (!string.IsNullOrWhiteSpace(form.Password))
            {
                player.Password = Helper.HashPassword(form.Password);
            }

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!this.PlayerExists(id))
                {
                    return this.HttpBadRequest("Invalid PlayerId");
                }
                throw;
            }

            // Add or Update the CustomData
            player.AddOrUpdateCustomData(this._context, form.CustomData);

            return this.CreatedAtRoute("GetPlayer", new { id = player.Id }, player);
        }

        // POST: api/players
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddPlayer([FromBody] UserForm register)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            if (string.IsNullOrWhiteSpace(register.Username) && string.IsNullOrWhiteSpace(register.Email))
            {
                return this.HttpBadRequest("Either Username or Email is required.");
            }

            if (string.IsNullOrWhiteSpace(register.Password))
            {
                return this.HttpBadRequest("Password is required.");
            }

            var player = new Player();

            if (!string.IsNullOrWhiteSpace(register.Username))
            {
                if (await Player.ExistsUsername(this._context, register.Username))
                {
                    return this.HttpBadRequest("Player with this Username already exists.");
                }

                player.Username = register.Username;
            }

            if (!string.IsNullOrWhiteSpace(register.Email))
            {
                if (await Player.ExistsEmail(this._context, register.Email))
                {
                    return this.HttpBadRequest("Player with this Email already exists.");
                }

                player.Email = register.Email;
            }

            player.Password = Helper.HashPassword(register.Password);

            var session = new Session { Player = player };

            this._context.Sessions.Add(session);

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.PlayerExists(player.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            // Add or Update the CustomData
            player.AddOrUpdateCustomData(this._context, register.CustomData);

            return this.CreatedAtRoute("GetPlayer", new { id = session.Id }, session);
        }

        // DELETE: api/players/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeletePlayer([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var player = await this._context.Players.FindAsync(id);
            if (player == null)
            {
                return this.HttpBadRequest("Invalid PlayerId");
            }

            player.IsEnabled = false;

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!this.PlayerExists(id))
                {
                    return this.HttpNotFound("No Player Found.");
                }
                throw;
            }

            return this.Ok(player);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool PlayerExists(Guid id)
        {
            return this._context.Players.Count(e => e.Id == id) > 0;
        }
    }
}