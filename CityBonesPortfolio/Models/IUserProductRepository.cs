namespace CityBonesPortfolio.Models
{
    public interface IUserProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
    }
}
