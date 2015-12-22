﻿using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGAControllers.Controllers
{
	[Produces("application/json")]
	[Route("api/groups")]
	public class GroupsController : Controller
	{
		private SocialGamificationAssetContext _context;

		public GroupsController(SocialGamificationAssetContext context)
		{
			_context = context;
		}

		// GET: api/groups
		[HttpGet]
		public IEnumerable<Group> GetGroups()
		{
			return _context.Groups;
		}

		// GET: api/groups/5
		[HttpGet("{id}", Name = "GetGroup")]
		public async Task<IActionResult> GetGroup([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Group group = await _context.Groups.SingleAsync(m => m.Id == id);

			if (group == null)
			{
				return HttpNotFound();
			}

			return Ok(group);
		}

		// PUT: api/groups/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutGroup([FromRoute] Guid id, [FromBody] Group group)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			if (id != group.Id)
			{
				return HttpBadRequest();
			}

			_context.Entry(group).State = System.Data.Entity.EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!GroupExists(id))
				{
					return HttpNotFound();
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

			return CreatedAtRoute("GetGroup", new { id = group.Id }, group);
		}

		// DELETE: api/groups/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteGroup([FromRoute] Guid id)
		{
			if (!ModelState.IsValid)
			{
				return HttpBadRequest(ModelState);
			}

			Group group = await _context.Groups.SingleAsync(m => m.Id == id);
			if (group == null)
			{
				return HttpNotFound();
			}

			_context.Groups.Remove(group);
			await _context.SaveChangesAsync();

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
