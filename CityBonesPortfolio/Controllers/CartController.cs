using CityBonesPortfolio.Helpers;
using CityBonesPortfolio.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityBonesPortfolio.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            var cart = CartManager.GetCart(HttpContext.Session);
            return View(cart);
        }

        [HttpPost]
        public IActionResult Add(int id, string name, decimal price, string returnUrl)
        {
           CartManager.AddToCart(HttpContext.Session, new CartItem
           {
               ProductId = id,
               ProductName = name,
               Price = price,
               Quantity = 1
           });

            if(!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }


            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            CartManager.RemoveFromCart(HttpContext.Session, id);
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            CartManager.ClearCart(HttpContext.Session);
            return RedirectToAction("Index");
        }

        //public IActionResult Cart()
        //{
        //    var cart = CartManager.GetCart(HttpContext.Session);
        //    return View(cart); // This looks for Views/Checkout/Cart.cshtml
        //}
    }
}
