using CityBonesPortfolio.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityBonesPortfolio.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Checkout()
        {
            var cartItem = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartItem) 
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(cartItem);
            
            return View("Cart", cart);
        }

        [HttpPost]
        public IActionResult ProcessCheckout()
        {
            return RedirectToAction("Confirmation");

            //for later save order, clear cart and redirect
        }

        public IActionResult Confirmation()
        {
            return View(); //make a thank you page or something
        }
    }
}
