namespace CityBonesPortfolio.Models
{
    public class Market
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }

        public byte[]? Image { get; set; }

        public string? ImageFileName { get; set; }  

        public string? ContentType { get; set; }
    }
}
