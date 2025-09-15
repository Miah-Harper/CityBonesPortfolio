using CityBonesPortfolio.Models;

namespace CityBonesPortfolio.Helpers
{
    public class CartManager
    {
        private const string CartSessionKey = "Cart";

        public static List<CartItem> GetCart(ISession session)
        {
           var cart = session.GetObjectFromJson<List<CartItem>>(CartSessionKey);

            if(cart == null)
            {
                cart = new List<CartItem>();
                session.SetObjectAsJson(CartSessionKey, cart);
            }
            return cart;
        }

        public static void AddToCart(ISession session, CartItem item)
        {
            var cart = GetCart(session);
            var existingItem = cart.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }
            session.SetObjectAsJson(CartSessionKey, cart);
        }

        public static void RemoveFromCart(ISession session, int productId)
        {
            var cart = GetCart(session);
            var itemToRemove = cart.FirstOrDefault(i => i.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                session.SetObjectAsJson(CartSessionKey, cart);
            }
        }

        public static void ClearCart(ISession session)
        {
            session.SetObjectAsJson(CartSessionKey, new List<CartItem>());
        }

        public static int GetCartCount(ISession session)
        {
            var cart = GetCart(session);
            return cart.Sum(i => i.Quantity);
        }
    }
}
