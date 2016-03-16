using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

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
        [ResponseType(typeof(IList<Group>))]
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
        [ResponseType(typeof(IList<Group>))]
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
        [ResponseType(typeof(Group))]
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
        [HttpPut("{id:Guid}", Name = "UpdateGroup")]
        [ResponseType(typeof(Group))]
        public async Task<IActionResult> UpdateGroup([FromRoute] Guid id, [FromBody] GroupFrom form)
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

            if (string.IsNullOrWhiteSpace(form.Name))
            {
                return Helper.HttpBadRequest("Group name is required.");
            }

            _context.Entry(group).State = EntityState.Modified;

            if (form.Name != group.Username)
            {
                if (await Group.ExistsUsername(_context, form.Name))
                {
                    return Helper.HttpBadRequest("Group with this name already exists.");
                }

                group.Username = form.Name;
            }

            if (form.Type != group.Type)
            {
                group.Type = form.Type;
            }

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return Ok(group);
        }

        // PUT: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8/players/936da01f-9abd-4d9d-80c7-02af85c822a8/add
        // PUT: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8/players/936da01f-9abd-4d9d-80c7-02af85c822a8/remove
        // PUT: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8/players/936da01f-9abd-4d9d-80c7-02af85c822a8/admin
        [HttpPut("{id:Guid}/players/{playerId:Guid}/{target?}", Name = "UpdateGroupPlayer")]
        [ResponseType(typeof(Group))]
        public async Task<IActionResult> UpdateGroupPlayer(
            [FromRoute] Guid id,
            [FromRoute] Guid playerId,
            [FromRoute] string target = "add")
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

            var player = await _context.Players.Where(p => p.Id.Equals(playerId)).FirstOrDefaultAsync();
            if (player == null)
            {
                return Helper.HttpNotFound("No such Player found.");
            }

            _context.Entry(group).State = EntityState.Modified;

            switch (target.ToLower())
            {
                case "add":
                    if (group.Players.Contains(player))
                    {
                        return Helper.HttpBadRequest($"Player with Id {playerId} already exists in the Group.");
                    }

                    // Add Player
                    group.Players.Add(player);

                    break;
                case "remove":
                    if (!group.Players.Contains(player))
                    {
                        return Helper.HttpBadRequest($"No Player with Id {playerId} exists in the Group.");
                    }

                    // Remove Player
                    group.Players.Remove(player);

                    break;
                case "admin":
                    if (group.AdminId != session.Player.Id)
                    {
                        return Helper.HttpUnauthorized($"You are not the Admin of this Group.");
                    }

                    if (!group.Players.Contains(player))
                    {
                        return Helper.HttpBadRequest($"No Player with Id {playerId} exists in the Group.");
                    }

                    // Make Admin
                    if (group.AdminId == player.Id)
                    {
                        return Helper.HttpBadRequest($"You are already Admin of this Group.");
                    }

                    group.Admin = player;

                    break;
                default:
                    return Helper.HttpBadRequest($"'{target}' is not a valid Action.");
            }

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return Ok(group);
        }

        // POST: api/groups
        [HttpPost("", Name = "CreateGroup")]
        [ResponseType(typeof(Group))]
        public async Task<IActionResult> CreateGroup([FromBody] GroupFrom form)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(form.Name))
            {
                return Helper.HttpBadRequest("Group name is required.");
            }

            if (await Group.ExistsUsername(_context, form.Name))
            {
                return Helper.HttpBadRequest("Group with this name already exists.");
            }

            var group = new Group { Username = form.Name, Type = form.Type, AdminId = session.Player.Id };

            if (form.Players == null || form.Players.Count < 1)
            {
                return Helper.HttpBadRequest("Group requires minimum 1 Player.");
            }

            IList<Player> players = new List<Player>();
            foreach (var playerId in form.Players)
            {
                var player = await _context.Players.FindAsync(playerId);
                if (player == null)
                {
                    return Helper.HttpNotFound($"No Player with Id {playerId} exists.");
                }

                players.Add(player);
            }

            group.Players = players;

            _context.Groups.Add(group);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetGroupInfo", new { id = group.Id }, group);
        }

        // DELETE: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id:Guid}", Name = "DisableGroup")]
        [ResponseType(typeof(Group))]
        public async Task<IActionResult> DisableGroup([FromRoute] Guid id)
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