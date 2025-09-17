namespace CityBonesPortfolio.Models
{
    public class CartItem
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }

        public int? UserId { get; set; }
        public string? ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Total => Price * Quantity;

        public string ImageUrl { get; set; }

        public string ContentType { get; set; }

        public byte[] ImageData { get; set; }

    }
}
