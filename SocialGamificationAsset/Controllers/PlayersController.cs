using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    /// <summary>
    /// </summary>
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

        // GET: api/players/whoami
        /// <summary>
        ///     Get <see cref="Player" /> Details
        /// </summary>
        /// <param name="id">GUID of <see cref="Player" /></param>
        /// <returns>
        /// </returns>
        [HttpGet("")]
        [HttpGet("whoami", Name = "WhoAmI")]
        [ResponseType(typeof(Player))]
        public async Task<IActionResult> WhoAmI()
        {
            if (session?.Player != null)
            {
                return Ok(session.Player);
            }

            return HttpNotFound("No Player found.");
        }

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
                return HttpBadRequest(ModelState);
            }

            var player = await _context.Players.Where(p => p.Id.Equals(id)).FirstOrDefaultAsync();
            if (player == null)
            {
                return HttpNotFound("No such Player found.");
            }

            return Ok(player);
        }

        // GET: api/players/936da01f-9abd-4d9d-80c7-02af85c822a8/achievements
        /// <summary>
        ///     Get <see cref="Player" /> 's Achievements
        /// </summary>
        /// <param name="id">GUID of <see cref="Player" /></param>
        /// <returns>
        /// </returns>
        [HttpGet("{id:Guid}/achievements", Name = "GetPlayerAchievements")]
        [ResponseType(typeof(IList<Achievement>))]
        public async Task<IActionResult> GetPlayerAchievements([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var query = _context.Players.Where(p => p.Id.Equals(id));

            var player = await query.FirstOrDefaultAsync();
            if (player == null)
            {
                return HttpNotFound("No such Player found.");
            }

            var achievements = await query.Include(p => p.Achievements).Select(p => p.Achievements).ToListAsync();

            return Ok(achievements);
        }

        // GET: api/players/936da01f-9abd-4d9d-80c7-02af85c822a8/goals
        /// <summary>
        ///     Get <see cref="Player" /> 's Goals
        /// </summary>
        /// <param name="id">GUID of <see cref="Player" /></param>
        /// <returns>
        /// </returns>
        [HttpGet("{id:Guid}/goals", Name = "GetPlayerGoals")]
        [ResponseType(typeof(IList<ActorGoal>))]
        public async Task<IActionResult> GetPlayerGoals([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var query = _context.Players.Where(p => p.Id.Equals(id));

            var player = await query.FirstOrDefaultAsync();
            if (player == null)
            {
                return HttpNotFound("No such Player found.");
            }

            IList<ActorGoal> goals =
                await _context.ActorGoal.Where(g => g.ActorId.Equals(session.Player.Id)).ToListAsync();

            return Ok(goals);
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
                return HttpBadRequest(ModelState);
            }

            var player = await _context.Players.Where(p => p.Id.Equals(id)).FirstOrDefaultAsync();

            if (player == null)
            {
                return HttpNotFound("No Player Found.");
            }

            _context.Entry(player).State = EntityState.Modified;

            if (!string.IsNullOrWhiteSpace(form.Username) && player.Username != form.Username)
            {
                if (await Player.ExistsUsername(_context, form.Username))
                {
                    return HttpBadRequest("Player with this Username already exists.");
                }

                player.Username = form.Username;
            }

            if (!string.IsNullOrWhiteSpace(form.Email) && player.Email != form.Email)
            {
                if (await Player.ExistsEmail(_context, form.Email))
                {
                    return HttpBadRequest("Player with this Email already exists.");
                }

                player.Email = form.Email;
            }

            if (!string.IsNullOrWhiteSpace(form.Password))
            {
                player.Password = Helper.HashPassword(form.Password);
            }

            await SaveChangesAsync();

            // Add or Update the CustomData
            await player.AddOrUpdateCustomData(_context, form.CustomData);

            return CreatedAtRoute("GetPlayer", new { id = player.Id }, player);
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
                return HttpBadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(register.Username) && string.IsNullOrWhiteSpace(register.Email))
            {
                return HttpBadRequest("Either Username or Email is required.");
            }

            if (string.IsNullOrWhiteSpace(register.Password))
            {
                return HttpBadRequest("Password is required.");
            }

            var player = new Player();

            if (!string.IsNullOrWhiteSpace(register.Username))
            {
                if (await Player.ExistsUsername(_context, register.Username))
                {
                    return HttpBadRequest("Player with this Username already exists.");
                }

                player.Username = register.Username;
            }

            if (!string.IsNullOrWhiteSpace(register.Email))
            {
                if (await Player.ExistsEmail(_context, register.Email))
                {
                    return HttpBadRequest("Player with this Email already exists.");
                }

                player.Email = register.Email;
            }

            player.Password = Helper.HashPassword(register.Password);

            var entity = new Session { Player = player };

            _context.Sessions.Add(entity);

            await SaveChangesAsync();

            // Add or Update the CustomData
            await player.AddOrUpdateCustomData(_context, register.CustomData);

            return CreatedAtRoute("GetPlayer", new { id = entity.Id }, entity);
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
                return HttpBadRequest(ModelState);
            }

            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return HttpBadRequest("Invalid PlayerId");
            }

            player.IsEnabled = false;

            await SaveChangesAsync();

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