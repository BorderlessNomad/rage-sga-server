using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        // GET: api/groups/all
        [HttpGet("all", Name = "GetAllGroups")]
        public IEnumerable<Group> GetAllGroups()
        {
            return this._context.Groups.Include(g => g.Players);
        }

        // GET: api/groups
        [HttpGet("", Name = "GetMyGroups")]
        public async Task<IActionResult> GetMyGroups()
        {
            var groups =
                await
                this._context.Players.Where(p => p.Id.Equals(this.session.Player.Id))
                    .Include(p => p.Groups)
                    .Select(p => p.Groups)
                    .FirstOrDefaultAsync();

            return this.Ok(groups);
        }

        // GET: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id:Guid}", Name = "GetGroupInfo")]
        public async Task<IActionResult> GetGroupInfo([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var group =
                await this._context.Groups.Where(g => g.Id.Equals(id)).Include(g => g.Players).FirstOrDefaultAsync();

            if (group == null)
            {
                return this.HttpNotFound("No Group Found.");
            }

            return this.Ok(group);
        }

        // PUT: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> PutGroup([FromRoute] Guid id, [FromBody] Group group)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            if (id != group.Id)
            {
                return this.HttpBadRequest("Id & Group.Id does not match.");
            }

            this._context.Entry(group).State = EntityState.Modified;

            if (group.Players != null && group.Players.Count != 0)
            {
                group.AddPlayers(this._context, group.Players);
            }

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!this.GroupExists(id))
                {
                    return this.HttpNotFound("No Group Found.");
                }
                throw;
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/groups
        [HttpPost]
        public async Task<IActionResult> PostGroup([FromBody] Group group)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            if (group.Players != null && group.Players.Count != 0)
            {
                group.AddPlayers(this._context, group.Players);
            }

            this._context.Groups.Add(group);
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.GroupExists(group.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return this.CreatedAtRoute("GetGroupInfo", new { id = group.Id }, group);
        }

        // DELETE: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var group = await this._context.Groups.FindAsync(id);
            if (group == null)
            {
                return this.HttpNotFound("No Group Found.");
            }

            group.IsEnabled = false;

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!this.GroupExists(id))
                {
                    return this.HttpNotFound("No Group Found.");
                }
                throw;
            }

            return this.Ok(group);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool GroupExists(Guid id)
        {
            return this._context.Groups.Count(e => e.Id == id) > 0;
        }
    }
}