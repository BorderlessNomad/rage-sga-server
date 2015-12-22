using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;

namespace SocialGamificationAsset.Controllers
{
	public class ApiController : Controller
	{
		protected SocialGamificationAssetContext _context;

		public ApiController(SocialGamificationAssetContext context)
		{
			_context = context;
		}
	}
}
