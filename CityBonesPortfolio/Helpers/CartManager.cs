using CityBonesPortfolio.Models;
using System.Text.Json;

namespace CityBonesPortfolio.Helpers
{
    public class CartManager
    {
        private const string CartSessionKey = "Cart";
        private const string SavedSessionKey = "SavedItems";

        public static List<CartItem> GetCart(ISession session)
        {
            var data = session.GetString(CartSessionKey);
            return data == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(data);
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

        public static List<CartItem>GetSavedItems(ISession session)
        {
            var data = session.GetString(SavedSessionKey);
            return data == null? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(data);
        }

        public static void SaveItem(ISession session, CartItem item)
        {
            var saved = GetSavedItems(session);
            saved.Add(item);
            session.SetString(SavedSessionKey, JsonSerializer.Serialize(saved));

        }

        public static void MoveToCart(ISession session, int itemId)
        {
            var saved = GetSavedItems(session);
            var item = saved.Find(i => i.CartId == itemId);

            if(item != null)
            {
                saved.Remove(item);
                session.SetString(SavedSessionKey, JsonSerializer.Serialize(saved));
                AddToCart(session, item);
            }
        }
    }
}
