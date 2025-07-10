namespace CityBonesPortfolio.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string? Name{ get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? FileName{ get; set; }

        public string? ContentType { get; set; }
        public byte[] ImageData { get; set; }

    }
}
