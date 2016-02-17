using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

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

        /// <exception cref="System.Web.Http.HttpResponseException">
        ///     Throws DB Exception when <see cref="DbUpdateException" /> (or for
        ///     any member of DbUpdateException) is raised.
        /// </exception>
        protected async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw Helper.ApiException(e);
            }
        }
    }
}