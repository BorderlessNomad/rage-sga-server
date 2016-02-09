using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/roles")]
    public class RolesController : ApiController
    {
        public RolesController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/roles
        [HttpGet]
        public IEnumerable<Role> GetRole()
        {
            return this._context.Roles;
        }

        // GET: api/roles/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id:Guid}", Name = "GetRole")]
        public async Task<IActionResult> GetRole([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var role = await this._context.Roles.FindAsync(id);

            if (role == null)
            {
                return this.HttpNotFound("No such Role found.");
            }

            return this.Ok(role);
        }

        // POST: api/roles
        [HttpPost("", Name = "AddRole")]
        [ResponseType(typeof(Role))]
        public async Task<IActionResult> AddRole([FromBody] Role role)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var checkRole = await this._context.Roles.Where(r => r.Name.Equals(role.Name)).FirstOrDefaultAsync();
            if (checkRole != null)
            {
                return this.HttpBadRequest("Role '" + checkRole.Name + "' already exists.");
            }

            this._context.Roles.Add(role);
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.RoleExists(role.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return this.CreatedAtRoute("GetRole", new { id = role.Id }, role);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool RoleExists(Guid id)
        {
            return this._context.Roles.Count(e => e.Id == id) > 0;
        }
    }
}