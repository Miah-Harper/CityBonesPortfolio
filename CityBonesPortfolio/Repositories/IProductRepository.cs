using CityBonesPortfolio.Models;

namespace CityBonesPortfolio.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
        Task<ProductViewModel> GetProductByIdAsync(int id);
        Task AddProductAsync(ProductViewModel product);

    }
}
