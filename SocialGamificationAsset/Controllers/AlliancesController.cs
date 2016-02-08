using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/alliances")]
    public class AlliancesController : ApiController
    {
        public AlliancesController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/alliances/all
        [HttpGet]
        [Route("all", Name = "GetAllAlliances")]
        public async Task<IActionResult> GetAllAlliances()
        {
            if (this.session == null || this.session.Player == null)
            {
                return this.HttpNotFound("Invalid Session.");
            }

            IList<Actor> alliances = await this.session.Player.Alliances(this._context)
                                               .ToListAsync();

            return this.Ok(alliances);
        }

        // GET: api/alliances
        // GET: api/alliances/accepted
        // GET: api/alliances/pending
        // GET: api/alliances/declined
        [HttpGet]
        [Route("{state?}", Name = "GetMyAlliances")]
        public async Task<IActionResult> GetMyAlliances([FromRoute] string state = "accepted")
        {
            if (this.session == null || this.session.Player == null)
            {
                return this.HttpNotFound("Invalid Session.");
            }

            var allianceshipStatus = AllianceState.Accepted;
            if (state == "pending")
            {
                allianceshipStatus = AllianceState.Pending;
            }
            else if (state == "declined")
            {
                allianceshipStatus = AllianceState.Declined;
            }

            IList<Actor> alliances = await this.session.Player.Alliances(this._context, allianceshipStatus)
                                               .ToListAsync();

            return this.Ok(alliances);
        }

        // GET: api/alliances/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet]
        [Route("{actorId:Guid}", Name = "GetActorAlliances")]
        public async Task<IActionResult> GetActorAlliances([FromRoute] Guid actorId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var actor = await this._context.Actors.Where(a => a.Id.Equals(actorId))
                                  .FirstOrDefaultAsync();

            if (actor == null)
            {
                return this.HttpNotFound("No Actor Found.");
            }

            IList<Actor> alliances = await actor.Alliances(this._context)
                                                .ToListAsync();

            return this.Ok(alliances);
        }

        // PUT: api/alliances/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut]
        [Route("{allianceId:Guid}")]
        public async Task<IActionResult> AddAlliance([FromRoute] Guid allianceId)
        {
            if (this.session == null || this.session.Player == null)
            {
                return this.HttpBadRequest("Error with your session.");
            }

            var actor = await this._context.Actors.Where(a => a.Id.Equals(allianceId))
                                  .FirstOrDefaultAsync();

            if (actor == null)
            {
                return this.HttpNotFound("No such Actor found.");
            }

            var alliance = await this._context.Alliances.Where(Alliance.IsAlliance(allianceId, this.session.Player.Id))
                                     .FirstOrDefaultAsync();

            if (alliance != null)
            {
                if (alliance.State != AllianceState.Accepted)
                {
                    return this.HttpBadRequest("Alliance Request already sent.");
                }

                return this.HttpBadRequest("You are already alliance with this Actor.");
            }

            var newAlliance = new Alliance { RequesterId = this.session.Player.Id, RequesteeId = allianceId };

            this._context.Alliances.Add(newAlliance);
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }

            return this.Ok(newAlliance);
        }

        // Delete: api/alliances/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete]
        [Route("{allianceId:Guid}")]
        public async Task<IActionResult> Unalliance([FromRoute] Guid allianceId)
        {
            if (this.session == null || this.session.Player == null)
            {
                return this.HttpBadRequest("Error with your session.");
            }

            var actor = await this._context.Actors.Where(a => a.Id.Equals(allianceId))
                                  .FirstOrDefaultAsync();

            if (actor == null)
            {
                return this.HttpNotFound("No such Actor found.");
            }

            var alliance = await this._context.Alliances.Where(Alliance.IsAlliance(allianceId, this.session.Player.Id))
                                     .FirstOrDefaultAsync();

            if (alliance == null)
            {
                return this.HttpBadRequest("Alliance is not in list.");
            }

            this._context.Alliances.Remove(alliance);
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw e;
            }

            return this.Ok("Alliance Removed fromt the list.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool AllianceExists(Guid id)
        {
            return this._context.Alliances.Count(e => e.Id == id) > 0;
        }
    }
}