using Microsoft.AspNetCore.Mvc;

namespace CityBonesPortfolio.Controllers
{
    public class AboutMe : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
