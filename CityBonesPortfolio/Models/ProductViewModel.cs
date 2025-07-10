using System.ComponentModel.DataAnnotations;

namespace CityBonesPortfolio.Models
{
    public class ProductViewModel
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01,99999)]
        public decimal Price { get; set; }
        
        [Required]
        public IFormFile Image { get; set; }
    }
}
