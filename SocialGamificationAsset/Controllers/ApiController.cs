using System;
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
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        public Session session => GetSession();

        protected Session GetSession()
        {
            return _session ?? (_session = HttpContext.Session.GetObjectFromJson<Session>("__session"));
        }

        /// <summary>
        ///     Asynchronously save data
        /// </summary>
        /// <returns>
        ///     JsonErrorContentResult if <see cref="DbUpdateException" /> exception
        ///     occurs
        /// </returns>
        protected async Task<ContentResult> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return Helper.JsonErrorContentResult(GetExceptionString(e));
            }

            return null;
        }

        protected string GetExceptionString(Exception e)
        {
            if (e.Message == "An error occurred while updating the entries. See the inner exception for details.")
            {
                return GetExceptionString(e.InnerException);
            }

            return e.Message;
        }
    }
}