using Microsoft.AspNet.Mvc;
using SocialGamificationAsset.Models;
using SocialGamificationAsset.Policies;

namespace SocialGamificationAsset.Controllers
{
	[Produces("application/json")]
	[ServiceFilter(typeof(ISessionAuthorizeFilter))]
	public class ApiController : Controller
	{
		protected SocialGamificationAssetContext _context;

		protected Session _session;

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

		public ApiController(SocialGamificationAssetContext context)
		{
			_context = context;
		}
	}
}
