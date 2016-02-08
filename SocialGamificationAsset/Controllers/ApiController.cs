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
            this._context = context;
        }

        public Session session => this.GetSession();

        protected Session GetSession()
        {
            return this._session ?? (this._session = this.HttpContext.Session.GetObjectFromJson<Session>("__session"));
        }
    }
}