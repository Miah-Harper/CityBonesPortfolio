using Microsoft.AspNetCore.Mvc;

namespace CityBonesPortfolio.Controllers
{
    public class MarketsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
