using System;
using System.Collections.Generic;

using Microsoft.AspNet.Mvc;

using SocialGamificationAsset.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialGamificationAsset.Controllers
{
    [Produces("application/json")]
    [Route("api/server")]
    public class ServerInfosController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            var result = new Dictionary<string, string>
                         {
                             ["Version"] = ServerSetting.ServerVersion,
                             ["Time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                             ["Success"] = "true"
                         };

            return Ok(result);
        }
    }
}