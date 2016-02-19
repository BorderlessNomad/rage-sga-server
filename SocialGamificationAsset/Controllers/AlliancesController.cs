using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            if (session?.Player == null)
            {
                return Helper.HttpNotFound("Invalid Session.");
            }

            IList<Actor> alliances = await session.Player.Alliances(_context).ToListAsync();

            return Ok(alliances);
        }

        // GET: api/alliances
        // GET: api/alliances/accepted
        // GET: api/alliances/pending
        // GET: api/alliances/declined
        [HttpGet]
        [Route("{state?}", Name = "GetAlliancesWithState")]
        public async Task<IActionResult> GetAlliancesWithState([FromRoute] string state = "accepted")
        {
            if (session?.Player == null)
            {
                return Helper.HttpNotFound("Invalid Session.");
            }

            var allianceStatus = AllianceState.Accepted;
            if (state == "pending")
            {
                allianceStatus = AllianceState.Pending;
            }
            else if (state == "declined")
            {
                allianceStatus = AllianceState.Declined;
            }

            IList<Actor> alliances = await session.Player.Alliances(_context, allianceStatus).ToListAsync();

            return Ok(alliances);
        }

        // GET: api/alliances/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet]
        [Route("{actorId:Guid}", Name = "GetActorAlliances")]
        public async Task<IActionResult> GetActorAlliances([FromRoute] Guid actorId)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var actor = await _context.Actors.FindAsync(actorId);

            if (actor == null)
            {
                return Helper.HttpNotFound("No Actor found.");
            }

            IList<Actor> alliances = await actor.Alliances(_context).ToListAsync();

            return Ok(alliances);
        }

        // PUT: api/alliances/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut]
        [Route("{allianceId:Guid}")]
        public async Task<IActionResult> AddAlliance([FromRoute] Guid allianceId)
        {
            if (session?.Player == null)
            {
                return Helper.HttpBadRequest("Error with your session.");
            }

            var actor = await _context.Actors.FindAsync(allianceId);

            if (actor == null)
            {
                return Helper.HttpNotFound("No such Actor found.");
            }

            var alliance =
                await _context.Alliances.Where(Alliance.IsAlliance(allianceId, session.Player.Id)).FirstOrDefaultAsync();

            if (alliance != null)
            {
                if (alliance.State != AllianceState.Accepted)
                {
                    return Helper.HttpBadRequest("Alliance Request already sent.");
                }

                return Helper.HttpBadRequest("You are already alliance with this Actor.");
            }

            var newAlliance = new Alliance { RequesterId = session.Player.Id, RequesteeId = allianceId };

            await SaveChangesAsync();

            return Ok(newAlliance);
        }

        // Delete: api/alliances/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete]
        [Route("{allianceId:Guid}")]
        public async Task<IActionResult> Unalliance([FromRoute] Guid allianceId)
        {
            if (session?.Player == null)
            {
                return Helper.HttpBadRequest("Error with your session.");
            }

            var actor = await _context.Actors.FindAsync(allianceId);

            if (actor == null)
            {
                return Helper.HttpNotFound("No such Actor found.");
            }

            var alliance =
                await _context.Alliances.Where(Alliance.IsAlliance(allianceId, session.Player.Id)).FirstOrDefaultAsync();

            if (alliance == null)
            {
                return Helper.HttpBadRequest("Alliance is not in list.");
            }

            _context.Alliances.Remove(alliance);

            await SaveChangesAsync();

            return Ok("Alliance Removed from the list.");
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