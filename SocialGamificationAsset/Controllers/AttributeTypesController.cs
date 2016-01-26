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
	[Route("api/attributes/types")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class AttributeTypesController : Controller
	{
		private SocialGamificationAssetContext _context;

		public AttributeTypesController(SocialGamificationAssetContext context)
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

		// GET: api/attributes/types
		[HttpGet]
		public IEnumerable<AttributeType> GetAttributeType()
		{
			return _context.AttributeTypes;
		}

		// GET: api/attributes/types/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpGet("{id}", Name = "GetAttributeType")]
		public async Task<IActionResult> GetAttributeType([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			AttributeType test = await _context.AttributeTypes.FindAsync(id);

			if (test == null)
			{
				return HttpNotFound();
			}

			return Ok(test);
		}

		// PUT: api/attributes/types/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpPut("{id}")]
		public async Task<IActionResult> PutAttributeType([FromRoute] Guid id, [FromBody] AttributeType test)
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
				if (!AttributeTypeExists(id))
				{
					return HttpNotFound();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetAttributeType", new { id = test.Id }, test);
		}

		// POST: api/attributes/types
		[HttpPost]
		[ResponseType(typeof(AttributeType))]
		public async Task<IActionResult> PostAttributeType([FromBody] AttributeType test)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			_context.AttributeTypes.Add(test);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (AttributeTypeExists(test.Id))
				{
					return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
				}
				else
				{
					throw;
				}
			}

			return CreatedAtRoute("GetAttributeType", new { id = test.Id }, test);
		}

		// DELETE: api/attributes/types/936da01f-9abd-4d9d-80c7-02af85c822a8
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAttributeType([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			AttributeType test = await _context.AttributeTypes.FindAsync(id);
			if (test == null)
			{
				return HttpNotFound();
			}

			_context.AttributeTypes.Remove(test);
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

		private bool AttributeTypeExists(Guid id)
		{
			return _context.AttributeTypes.Count(e => e.Id == id) > 0;
		}
	}
}
