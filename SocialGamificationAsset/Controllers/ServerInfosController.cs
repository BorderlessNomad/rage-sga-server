using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[Route("api/server")]
	public class ServerInfosController : Controller
	{
		// POST: api/values
		[HttpPost]
		public IActionResult Get()
		{
			Dictionary<string, string> result = new Dictionary<string, string>()
			{
				["Version"] = ServerSetting.ServerVersion,
				["Time"] = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss")
			};

			return Ok(result);
		}
	}
}
