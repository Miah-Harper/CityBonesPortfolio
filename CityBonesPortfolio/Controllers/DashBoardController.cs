using CityBonesPortfolio.Helpers;
using CityBonesPortfolio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace CityBonesPortfolio.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            var cart = CartManager.GetCart(HttpContext.Session) ?? new List<CartItem>();

            var savedItems = CartManager.GetSavedItems(HttpContext.Session) ?? new List<CartItem>();

            var model = new DashBoard
            {
                CartItems = cart,
                SavedItems = savedItems
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            CartManager.RemoveFromCart(HttpContext.Session, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MoveToCart(int id)
        {
            CartManager.MoveToCart(HttpContext.Session, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SaveForLater(int id)
        {
            var cart = CartManager.GetCart(HttpContext.Session);
            var item = cart.Find(x => x.ProductId == id);
            if (item != null)
            {
                CartManager.RemoveFromCart(HttpContext.Session, id);
                CartManager.SaveItem(HttpContext.Session, item);
            }
            return RedirectToAction("Index");
        }
    }
}
