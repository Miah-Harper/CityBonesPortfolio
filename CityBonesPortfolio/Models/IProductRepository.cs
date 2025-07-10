namespace CityBonesPortfolio.Models
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
        Task<ProductViewModel> GetProductByIdAsync(int id);
        Task AddProductAsync(ProductViewModel product);

    }
}
