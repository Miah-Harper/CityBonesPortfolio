using CityBonesPortfolio.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using System.Text.Json;
using Stripe;
using Stripe.Checkout;

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

        [HttpPost]
        [HttpPost]
        public IActionResult Pay()
        {
            var cart = GetCartFromSession(); // your method to get cart items

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = cart.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = item.Price * 100, // Stripe expects cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName
                        },
                    },
                    Quantity = item.Quantity,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Checkout", null, Request.Scheme),
                CancelUrl = Url.Action("Cancel", "Checkout", null, Request.Scheme)
            };

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            return Redirect(session.Url);
        }


        public IActionResult Success()
        {
            // Clear the cart after successful payment
            HttpContext.Session.Remove("Cart");
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }

        private List<CartItem> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }
    }
}
