using SocialGamificationAsset.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGamificationAsset.Controllers
{
	public class ApiController
	{
		protected SocialGamificationAssetContext _context;

		public ApiController(SocialGamificationAssetContext context)
		{
			_context = context;
		}
	}
}
