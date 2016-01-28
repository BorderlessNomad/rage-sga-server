using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace SocialGamificationAsset.Controllers
{
	[Route("api/roles")]
	public class RolesController : ApiController
	{
		public RolesController(SocialGamificationAssetContext context) : base(context)
		{
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

			Role role = await _context.Roles.FindAsync(id);

			if (role == null)
			{
				return HttpNotFound("No such Role found.");
			}

			return Ok(role);
		}

		// POST: api/roles
		[HttpPost]
		[ResponseType(typeof(Role))]
		public async Task<IActionResult> AddRole([FromBody] Role role)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Role checkRole = await _context.Roles.Where(r => r.Name.Equals(role.Name)).FirstOrDefaultAsync();
			if (checkRole != null)
			{
				return HttpBadRequest("Role '" + checkRole.Name + "' already exists.");
			}

			_context.Roles.Add(role);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (RoleExists(role.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetRole", new { id = role.Id }, role);
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
