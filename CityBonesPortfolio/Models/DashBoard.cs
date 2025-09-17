namespace CityBonesPortfolio.Models
{
    public class DashBoard
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<CartItem> SavedItems { get; set; } = new List<CartItem>();
    }
}
