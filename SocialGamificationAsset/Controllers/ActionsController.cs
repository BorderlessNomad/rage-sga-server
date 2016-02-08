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

using Action = SocialGamificationAsset.Models.Action;

namespace SocialGamificationAsset.Controllers
{
    public class ActionsController : ApiController
    {
        public ActionsController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/actions
        [HttpGet]
        public IEnumerable<Action> GetAction()
        {
            return this._context.Actions;
        }

        // GET: api/actions/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id}", Name = "GetAction")]
        public async Task<IActionResult> GetAction([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var test = await this._context.Actions.FindAsync(id);

            if (test == null)
            {
                return this.HttpNotFound();
            }

            return this.Ok(test);
        }

        // PUT: api/actions/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAction([FromRoute] Guid id, [FromBody] Action test)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            if (id != test.Id)
            {
                return this.HttpBadRequest();
            }

            this._context.Entry(test)
                .State = EntityState.Modified;

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!this.ActionExists(id))
                {
                    return this.HttpNotFound();
                }
                throw;
            }

            return this.CreatedAtRoute("GetAction", new { id = test.Id }, test);
        }

        // POST: api/actions
        [HttpPost]
        [ResponseType(typeof(Action))]
        public async Task<IActionResult> PostAction([FromBody] Action test)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            this._context.Actions.Add(test);
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (this.ActionExists(test.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return this.CreatedAtRoute("GetAction", new { id = test.Id }, test);
        }

        // DELETE: api/actions/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAction([FromRoute] Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.HttpBadRequest(this.ModelState);
            }

            var test = await this._context.Actions.FindAsync(id);
            if (test == null)
            {
                return this.HttpNotFound();
            }

            this._context.Actions.Remove(test);
            await this._context.SaveChangesAsync();

            return this.Ok(test);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._context.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool ActionExists(Guid id)
        {
            return this._context.Actions.Count(e => e.Id == id) > 0;
        }
    }
}