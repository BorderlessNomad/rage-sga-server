using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
    [Route("api/tests")]
    public class TestsController : ApiController
    {
        public TestsController(SocialGamificationAssetContext context)
            : base(context)
        {
        }

        // GET: api/tests
        /// <summary>
        ///     <para>
        ///         This method returns all available records from <see cref="Test" />
        ///     </para>
        ///     <para>Table</para>
        /// </summary>
        /// <returns>
        ///     All the test records which were found
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Test> GetTest()
        {
            return _context.Tests;
        }

        // GET: api/tests/936da01f-9abd-4d9d-80c7-02af85c822a8
        /// <summary>
        ///     This method returns specific record from <see cref="Test" /> Table
        /// </summary>
        /// <param name="id">
        ///     <see cref="Guid" />
        /// </param>
        /// <returns>
        ///     <see cref="Test" /> record that was found
        /// </returns>
        [HttpGet("{id}", Name = "GetTest")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTest([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var test = await _context.Tests.FindAsync(id);

            if (test == null)
            {
                return HttpNotFound();
            }

            return Ok(test);
        }

        // PUT: api/tests/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTest([FromRoute] Guid id, [FromBody] Test test)
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
                if (!TestExists(id))
                {
                    return HttpNotFound();
                }
                throw;
            }

            return CreatedAtRoute("GetTest", new { id = test.Id }, test);
        }

        // POST: api/tests
        [HttpPost]
        [ResponseType(typeof(Test))]
        public async Task<IActionResult> PostTest([FromBody] Test test)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.Tests.Add(test);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TestExists(test.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return CreatedAtRoute("GetTest", new { id = test.Id }, test);
        }

        // DELETE: api/tests/936da01f-9abd-4d9d-80c7-02af85c822a8
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTest([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                return HttpNotFound();
            }

            _context.Tests.Remove(test);
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

        private bool TestExists(Guid id)
        {
            return _context.Tests.Count(e => e.Id == id) > 0;
        }
    }
}