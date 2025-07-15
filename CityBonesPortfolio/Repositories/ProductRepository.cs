using CityBonesPortfolio.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace CityBonesPortfolio.Repositories
{
    public class ProductRepository : IProductRepository
    {

        private readonly string _connString;

        public ProductRepository(string connString)
        {
            _connString = connString;
        }

        private IDbConnection Connection => new MySqlConnection(_connString);

        public async Task<IEnumerable<ProductViewModel>> GetAllProductsAsync()
        {
            using var db = Connection;
            string sql = "SELECT Id, Name, Description, Price, ImageFileName FROM Product";
            var products = await db.QueryAsync<ProductViewModel>(sql);
            return products;
        }

        public async Task<ProductViewModel> GetProductByIdAsync(int id)
        {
            using var db = Connection;
            string sql = "SELECT Id, Name, Description, Price, ImageFileName FROM Product WHERE Id = @Id";

            return await db.QueryFirstOrDefaultAsync<ProductViewModel>(sql, new { Id = id });
        }

        public async Task AddProductAsync(ProductViewModel product)
        {
            using var db = Connection;
            string sql = @"INSERT INTO Product (Name, Description, Price, ImageFileName)
             VALUES (@Name, @Description, @Price, @ImageFileName)";
            await db.ExecuteAsync(sql, product);
        }


    }
}
