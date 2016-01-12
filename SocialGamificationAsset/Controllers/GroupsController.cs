using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/groups")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class GroupsController : Controller
	{
		private SocialGamificationAssetContext _context;

		private Session _session;

		public Session session
		{
			get { return GetSession(); }
		}

		protected Session GetSession()
		{
			if (_session == null)
			{
				_session = HttpContext.Session.GetObjectFromJson<Session>("__session");
			}

			return _session;
		}

		public GroupsController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		// GET: api/groups/all
		[HttpGet("all", Name = "GetAllGroups")]
		public IEnumerable<Group> GetAllGroups()
		{
			return _context.Groups.Include(g => g.Players);
		}

		// GET: api/groups
		[HttpGet("", Name = "GetMyGroups")]
		public async Task<IActionResult> GetMyGroups()
		{
			var groups = await _context.Players
				.Where(p => p.Id.Equals(session.Player.Id))
				.Include(p => p.Groups)
				.Select(p => p.Groups)
				.FirstOrDefaultAsync()
			;

			return Ok(groups);
		}

		// GET: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id:Guid}", Name = "GetGroupInfo")]
		public async Task<IActionResult> GetGroupInfo([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Group group = await _context.Groups.Where(g => g.Id.Equals(id)).Include(g => g.Players).FirstOrDefaultAsync();

			if (group == null)
			{
				return HttpNotFound("No Group Found.");
			}

			return Ok(group);
		}

		// PUT: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id:Guid}")]
		public async Task<IActionResult> PutGroup([FromRoute] Guid id, [FromBody] Group group)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (id != group.Id)
			{
				return HttpBadRequest("Id & Group.Id does not match.");
			}

			_context.Entry(group).State = EntityState.Modified;

			if (group.Players != null && group.Players.Count != 0)
			{
				group.AddPlayers(_context, group.Players);
			}

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!GroupExists(id))
				{
					return HttpNotFound("No Group Found.");
				}
				else
				{
					throw;
				}
			}

			return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
		}

		// POST: api/groups
		[HttpPost]
		public async Task<IActionResult> PostGroup([FromBody] Group group)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (group.Players != null && group.Players.Count != 0)
			{
				group.AddPlayers(_context, group.Players);
			}

			_context.Groups.Add(group);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (GroupExists(group.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetGroupInfo", new { id = group.Id }, group);
		}

		// DELETE: api/groups/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id:Guid}")]
		public async Task<IActionResult> DeleteGroup([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Group group = await _context.Groups.FindAsync(id);
			if (group == null)
			{
				return HttpNotFound("No Group Found.");
			}

			group.IsEnabled = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!GroupExists(id))
				{
					return HttpNotFound("No Group Found.");
				}
				else
				{
					throw;
				}
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

		private bool GroupExists(Guid id)
		{
			return _context.Groups.Count(e => e.Id == id) > 0;
		}
	}
}
