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

        public ApiController(SocialGamificationAssetContext context)
        {
            _context = context;
        }

        public Session session => GetSession();

        protected Session GetSession()
        {
            return _session ?? (_session = HttpContext.Session.GetObjectFromJson<Session>("__session"));
        }
    }
}