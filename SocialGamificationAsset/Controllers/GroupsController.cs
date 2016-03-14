using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/groups")]
    public class GroupsController : ApiController
    {
        public GroupsController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/groups
        [HttpGet("", Name = "GetMyGroups")]
        public async Task<IActionResult> GetMyGroups()
        {
            var groups =
                await
                _context.Players.Where(p => p.Id.Equals(session.Player.Id))
                        .Include(p => p.Groups)
                        .Select(p => p.Groups)
                        .FirstOrDefaultAsync();

            return Ok(groups);
        }

        // GET: api/groups/actor/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("actor/{id:Guid}", Name = "GetActorGroups")]
        public async Task<IActionResult> GetActorGroups([FromRoute] Guid id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return Helper.HttpNotFound("No such Player found.");
            }

            var groups =
                await
                _context.Players.Where(p => p.Id.Equals(player.Id))
                        .Include(p => p.Groups)
                        .Select(p => p.Groups)
                        .FirstOrDefaultAsync();

            return Ok(groups);
        }

        // GET: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id:Guid}", Name = "GetGroupInfo")]
        public async Task<IActionResult> GetGroupInfo([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var group = await _context.Groups.Where(g => g.Id.Equals(id)).Include(g => g.Players).FirstOrDefaultAsync();

            if (group == null)
            {
                return Helper.HttpNotFound("No Group found.");
            }

            return Ok(group);
        }

        // PUT: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> PutGroup([FromRoute] Guid id, [FromBody] Group group)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            _context.Entry(group).State = EntityState.Modified;

            if (group.Players != null && group.Players.Count != 0)
            {
                group.AddPlayers(_context, group.Players);
            }

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/groups
        [HttpPost]
        public async Task<IActionResult> PostGroup([FromBody] Group group)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            if (group.Players != null && group.Players.Count != 0)
            {
                group.AddPlayers(_context, group.Players);
            }

            _context.Groups.Add(group);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetGroupInfo", new { id = group.Id }, group);
        }

        // DELETE: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return Helper.HttpNotFound("No Group found.");
            }

            group.IsEnabled = false;

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return Ok(group);
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