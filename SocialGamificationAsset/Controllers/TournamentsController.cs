using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/tournaments")]
    public class TournamentsController : ApiController
    {
        public TournamentsController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/tournaments/all
        [HttpGet("all", Name = "GetAllTournaments")]
        public IEnumerable<Tournament> GetAllTournaments()
        {
            return this._context.Tournaments;
        }

        // GET: api/tournaments/owned
        [HttpGet("owned", Name = "GetOwnedTournaments")]
        public async Task<IActionResult> GetOwnedTournaments()
        {
            IList<Tournament> tournaments =
                await this._context.Tournaments.Where(t => t.OwnerId.Equals(this.session.Player.Id))
                          .ToListAsync();

            if (tournaments == null || !tournaments.Any())
            {
                return this.HttpNotFound("No Tournament Found.");
            }

            return this.Ok(tournaments);
        }

        // GET: api/tournaments
        // GET: api/tournaments/participated
        [HttpGet("", Name = "GetMyTournaments")]
        [HttpGet("participated", Name = "GetParticipatedTournaments")]
        public async Task<IActionResult> GetMyTournaments()
        {
            IList<Tournament> tournaments =
                await this._context.Tournaments.Where(t => t.OwnerId.Equals(this.session.Player.Id))
                          .ToListAsync();

            if (tournaments == null || !tournaments.Any())
            {
                return this.HttpNotFound("No Tournament Found.");
            }

            return this.Ok(tournaments);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool TournamentExists(Guid id)
        {
            return this._context.Tournaments.Count(e => e.Id.Equals(id)) > 0;
        }
    }
}