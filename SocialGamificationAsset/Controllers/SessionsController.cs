using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

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
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var playerSession =
                await this._context.Sessions.Include(s => s.Player).Where(s => s.Id.Equals(id)).FirstOrDefaultAsync();

            if (playerSession == null)
            {
                return this.HttpNotFound("No such Session found.");
            }

            return this.Ok(playerSession);
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
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            if (string.IsNullOrWhiteSpace(login.Username) && string.IsNullOrWhiteSpace(login.Email))
            {
                return this.HttpBadRequest("Either Username or Email is required.");
            }

            if (string.IsNullOrWhiteSpace(login.Password))
            {
                return this.HttpBadRequest("Password is required.");
            }

            IQueryable<Player> query = this._context.Players;

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
                return this.HttpNotFound("No such Player found.");
            }

            if (!Helper.ValidatePassword(login.Password, player.Password))
            {
                return new ContentResult
                       {
                           StatusCode = StatusCodes.Status401Unauthorized,
                           Content = "Invalid Login Details."
                       };
            }

            var playerSession = new Session { Player = player };

            this._context.Sessions.Add(playerSession);

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.SessionExists(playerSession.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return this.CreatedAtRoute("GetSession", new { id = playerSession.Id }, playerSession);
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
        public async Task<IActionResult> Logout([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var playerSession = await this._context.Sessions.FindAsync(id);
            if (playerSession == null)
            {
                return this.HttpNotFound("No such Session found.");
            }

            playerSession.IsExpired = true;

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.SessionExists(playerSession.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return this.Ok(playerSession);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool SessionExists(Guid id)
        {
            return this._context.Sessions.Count(e => e.Id == id) > 0;
        }
    }
}