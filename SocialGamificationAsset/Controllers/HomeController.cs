using Microsoft.AspNet.Mvc;

namespace SocialGamificationAsset.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/swagger/ui");
            // return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "RAGE Social Gamification Asset.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Our HQ Tech City";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}