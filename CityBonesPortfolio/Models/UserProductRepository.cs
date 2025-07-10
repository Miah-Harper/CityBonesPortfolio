
using Dapper;
using MySql.Data.MySqlClient;

namespace CityBonesPortfolio.Models
{
    public class UserProductRepository : IUserProductRepository
    {
        private readonly IConfiguration _config;

        public UserProductRepository(IConfiguration config)
        {
            _config = config;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            using var conn = new MySqlConnection(_config.GetConnectionString("citybones"));
            var products = await conn.QueryAsync<Product>("SELECT * FROM Product");
            return products;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            using var conn = new MySqlConnection(_config.GetConnectionString("citybones"));
            var product = await conn.QueryFirstOrDefaultAsync<Product>("SELECT * FROM Product WHERE Id = Id", 
                new {Id = id });
            return product;
        }
    }
}
