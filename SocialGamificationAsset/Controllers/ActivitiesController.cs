using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/activities")]
    public class ActivitiesController : ApiController
    {
        public ActivitiesController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/activities
        [HttpGet]
        public IEnumerable<Activity> GetActivity()
        {
            return _context.Activities;
        }

        // GET: api/activities/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpGet("{id}", Name = "GetActivity")]
        public async Task<IActionResult> GetActivity([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var activity = await _context.Activities.FindAsync(id);

            if (activity == null)
            {
                return Helper.HttpNotFound("No Activity found.");
            }

            return Ok(activity);
        }

        // PUT: api/activities/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActivity([FromRoute] Guid id, [FromBody] Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            if (id != activity.Id)
            {
                return Helper.HttpBadRequest("Invalid Activity Id.");
            }

            _context.Entry(activity).State = EntityState.Modified;

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetActivity", new { id = activity.Id }, activity);
        }

        // POST: api/activities
        [HttpPost]
        [ResponseType(typeof(Activity))]
        public async Task<IActionResult> PostActivity([FromBody] Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            _context.Activities.Add(activity);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return CreatedAtRoute("GetActivity", new { id = activity.Id }, activity);
        }

        // DELETE: api/activities/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Helper.HttpBadRequest(ModelState);
            }

            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return Helper.HttpNotFound("No Activity found.");
            }

            _context.Activities.Remove(activity);

            var error = await SaveChangesAsync();
            if (error != null)
            {
                return error;
            }

            return Ok(activity);
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