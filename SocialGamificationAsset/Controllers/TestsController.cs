using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/tests")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class TestsController : Controller
	{
		private SocialGamificationAssetContext _context;

		public TestsController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		// GET: api/tests
		/// <summary>
		/// This method returns all available records from Test Table
		/// </summary>
		/// <returns>All the test records which were found</returns>
		[HttpGet]
		public IEnumerable<Test> GetTest()
		{
			return _context.Tests;
		}

		// GET: api/tests/936DA01F-9ABD-4d9d-80C7-02AF85C822A8
		/// <summary>
		/// This method returns specific record from Test Table
		/// </summary>
		/// <param name="id">Guid</param>
		/// <returns>Test record that was found</returns>
		[HttpGet("{id}", Name = "GetTest")]
		[AllowAnonymous]
		public async Task<IActionResult> GetTest([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Test test = await _context.Tests.FindAsync(id);

			if (test == null)
			{
				return HttpNotFound();
			}

			return Ok(test);
		}

		// PUT: api/tests/936DA01F-9ABD-4d9d-80C7-02AF85C822A8
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

			_context.Entry(test).State = System.Data.Entity.EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!TestExists(id))
				{
					return HttpNotFound();
				}
				else
				{
					throw;
				}
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
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetTest", new { id = test.Id }, test);
		}

		// DELETE: api/tests/936DA01F-9ABD-4d9d-80C7-02AF85C822A8
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTest([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Test test = await _context.Tests.FindAsync(id);
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
