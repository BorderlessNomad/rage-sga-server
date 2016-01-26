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
using System.Web.Http.Description;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/roles")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class RolesController : Controller
	{
		private SocialGamificationAssetContext _context;

		public RolesController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

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

		// GET: api/roles
		[HttpGet]
		public IEnumerable<Role> GetRole()
		{
			return _context.Roles;
		}

		// GET: api/roles/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id}", Name = "GetRole")]
		public async Task<IActionResult> GetRole([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Role test = await _context.Roles.FindAsync(id);

			if (test == null)
			{
				return HttpNotFound();
			}

			return Ok(test);
		}

		// PUT: api/roles/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id}")]
		public async Task<IActionResult> PutRole([FromRoute] Guid id, [FromBody] Role test)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (id != test.Id)
			{
				return HttpBadRequest();
			}

			_context.Entry(test).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (!RoleExists(id))
				{
					return HttpNotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetRole", new { id = test.Id }, test);
		}

		// POST: api/roles
		[HttpPost]
		[ResponseType(typeof(Role))]
		public async Task<IActionResult> PostRole([FromBody] Role test)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.Roles.Add(test);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (RoleExists(test.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetRole", new { id = test.Id }, test);
		}

		// DELETE: api/roles/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteRole([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Role test = await _context.Roles.FindAsync(id);
			if (test == null)
			{
				return HttpNotFound();
			}

			_context.Roles.Remove(test);
			await _context.SaveChangesAsync();

			return Ok(test);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context.Dispose();
			}

			base.Dispose(disposing);
		}

		private bool RoleExists(Guid id)
		{
			return _context.Roles.Count(e => e.Id == id) > 0;
		}
	}
}
