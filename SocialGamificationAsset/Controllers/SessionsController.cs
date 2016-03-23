using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Helpers;
using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/sessions")]
    public class SessionsController : ApiController
    {
        public SessionsController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/sessions/936da01f-9abd-4d9d-80c7-02af85c822a8
        /// <summary>
        ///     Returns the specified <see cref="Session" />
        /// </summary>
        /// <param name="id">GUID of the <see cref="Session" /></param>
        /// <returns>
        ///     A <see cref="Session" /> record with an HTTP 200, or a string
        ///     message with an HTTP 400 or HTTP 404.
        /// </returns>
        /// <response code="200">
        ///     OK
        /// </response>
        /// <response code="400">
        ///     Bad Request
        /// </response>
        /// <response code="404">
        ///     Not Found
        /// </response>
        /// <response code="500">
        ///     Internal Server Error
        /// </response>
        [HttpGet("{id:Guid}", Name = "GetSession")]
        [ResponseType(typeof(Session))]
        [AllowAnonymous]
        public async Task<IActionResult> GetSession([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpResponseHelper.BadRequest(ModelState);
            }

            var playerSession =
                await _context.Sessions.Include(s => s.Player).Where(s => s.Id.Equals(id)).FirstOrDefaultAsync();

            if (playerSession == null)
            {
                return HttpResponseHelper.NotFound("No such Session found.");
            }

            return Ok(playerSession);
        }

        // POST: api/sessions
        /// <summary>
        ///     Create <see cref="Session" /> for given <see cref="Player" /> /
        ///     <see cref="SessionsController.Login" />
        /// </summary>
        /// <param name="login">
        ///     <see cref="Player" /> <see cref="SessionsController.Login" /> Info
        /// </param>
        /// <returns>
        ///     A <see cref="Session" /> record with an HTTP 200, or a string
        ///     message with an HTTP 400 or HTTP 404 or HTTP 409.
        /// </returns>
        /// <response code="200">
        ///     OK
        /// </response>
        /// <response code="400">
        ///     Bad Request
        /// </response>
        /// <response code="404">
        ///     Not Found
        /// </response>
        /// <response code="409">
        ///     Conflict
        /// </response>
        /// <response code="500">
        ///     Internal Server Error
        /// </response>
        [HttpPost]
        [ResponseType(typeof(Session))]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserForm login)
        {
            if (!ModelState.IsValid)
            {
                return HttpResponseHelper.BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(login.Username) && string.IsNullOrWhiteSpace(login.Email))
            {
                return HttpResponseHelper.BadRequest("Either Username or Email is required.");
            }

            if (string.IsNullOrWhiteSpace(login.Password))
            {
                return HttpResponseHelper.BadRequest("Password is required.");
            }

            IQueryable<Player> query = _context.Players;

            if (!string.IsNullOrWhiteSpace(login.Username))
            {
                query = query.Where(a => a.Username.Equals(login.Username));
            }

            if (!string.IsNullOrWhiteSpace(login.Email))
            {
                query = query.Where(a => a.Email.Equals(login.Email));
            }

            var player = await query.FirstOrDefaultAsync();

            if (player == null)
            {
                return HttpResponseHelper.NotFound("No such Player found.");
            }

            if (!PasswordHelper.ValidatePassword(login.Password, player.Password))
            {
                return HttpResponseHelper.Unauthorized("Invalid Login Details.");
            }

            var playerSession = new Session { Player = player };

            _context.Sessions.Add(playerSession);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetSession", new { id = playerSession.Id }, playerSession);
        }

        // DELETE: api/sessions/936da01f-9abd-4d9d-80c7-02af85c822a8
        /// <summary>
        ///     Delete a <see cref="Session" /> /
        ///     <see cref="SessionsController.Logout" />
        /// </summary>
        /// <param name="id">GUID of the <see cref="Session" /></param>
        /// <returns>
        ///     A <see cref="Session" /> record with an HTTP 200, or a string
        ///     message with an HTTP 400 or HTTP 404 or HTTP 409.
        /// </returns>
        /// <response code="200">
        ///     OK
        /// </response>
        /// <response code="400">
        ///     Bad Request
        /// </response>
        /// <response code="404">
        ///     Not Found
        /// </response>
        /// <response code="409">
        ///     Conflict
        /// </response>
        /// <response code="500">
        ///     Internal Server Error
        /// </response>
        [HttpDelete("{id:Guid}")]
        [ResponseType(typeof(Session))]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpResponseHelper.BadRequest(ModelState);
            }

            var playerSession = await _context.Sessions.FindAsync(id);
            if (playerSession == null)
            {
                return HttpResponseHelper.NotFound("No such Session found.");
            }

            playerSession.IsExpired = true;

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return Ok(playerSession);
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