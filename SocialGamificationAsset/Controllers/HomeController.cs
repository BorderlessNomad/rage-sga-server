using Microsoft.AspNet.Mvc;

namespace SocialGamificationAsset.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult About()
        {
            this.ViewData["Message"] = "RAGE Social Gamification Asset.";

            return this.View();
        }

        public IActionResult Contact()
        {
            this.ViewData["Message"] = "Our HQ Tech City";

            return this.View();
        }

        public IActionResult Error()
        {
            return this.View();
        }
    }
}