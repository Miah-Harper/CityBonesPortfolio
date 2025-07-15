using System.ComponentModel.DataAnnotations;

namespace CityBonesPortfolio.Models
{
    public class MarketViewModel
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Location { get; set; }

        [Required]
        public IFormFile Image { get; set; } //uploading

        public byte[]? ImageData { get; set; } //displaying

        public bool HasExistingImage { get; set; }

    }
}
