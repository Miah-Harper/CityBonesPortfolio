namespace CityBonesPortfolio.Models
{
    public class CartItem
    {
        public int CartId { get; set; }

        public string? ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
        
    }
}
