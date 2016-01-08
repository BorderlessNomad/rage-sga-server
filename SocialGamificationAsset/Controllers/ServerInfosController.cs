using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
			return Ok(new Hashtable() {
						{"Version", ServerSetting.ServerVersion},
						{"Time", DateTime.Now.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss")},
					});
		}
	}
}
