using CityBonesPortfolio.Models;

namespace CityBonesPortfolio.Repositories
{
    public interface IUserProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
    }
}
