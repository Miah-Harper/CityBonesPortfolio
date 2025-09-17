using CityBonesPortfolio.Helpers;
using CityBonesPortfolio.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace CityBonesPortfolio.Controllers
{
    public class CartController : Controller
    {
        private readonly IConfiguration _config;

        public CartController(IConfiguration config)
        {
            _config = config;
        }
        public IActionResult Index()
        {
            var cart = CartManager.GetCart(HttpContext.Session);
            return View(cart);
        }

        [HttpPost]
        public IActionResult Add(int id, string returnUrl)
        {
            using (var db = new MySql.Data.MySqlClient.MySqlConnection(
                 _config.GetConnectionString("citybones")))
            {
                // Pull full product record
                var product = db.QuerySingleOrDefault<Product>(
                    @"SELECT Id, Name AS Name, Price, ContentType, ImageData 
                    FROM Product
                    WHERE Id = @id", new { id });

                if (product == null)
                {
                    return RedirectToAction("Index", "Home"); // fallback
                }

                // Add to cart with image data
                CartManager.AddToCart(HttpContext.Session, new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    ContentType = product.ContentType,
                    ImageData = product.ImageData
                });
            }

            if (!string.IsNullOrEmpty(returnUrl))
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