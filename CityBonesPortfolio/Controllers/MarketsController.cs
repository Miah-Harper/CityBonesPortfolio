using CityBonesPortfolio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityBonesPortfolio.Controllers
{
    public class MarketsController : Controller
    {
        private readonly IMarketRepository _repo;

        public MarketsController(IMarketRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
           var markets = _repo.GetAllMarkets();
            return View(markets); //this is visible to all users
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add(Market market)
        {
            if(ModelState.IsValid)
            {
                _repo.AddMarket(market);
                return RedirectToAction("Index");

            }
            return View(market);
        }
    }
}
