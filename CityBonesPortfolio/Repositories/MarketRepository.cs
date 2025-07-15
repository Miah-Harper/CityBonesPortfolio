using CityBonesPortfolio.Models;
using Dapper;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Data;

namespace CityBonesPortfolio.Repositories
{
    public class MarketRepository : IMarketRepository
    {
        private readonly IDbConnection _conn;

        public MarketRepository(IConfiguration config)
        {
            _conn = new MySqlConnection(config.GetConnectionString("citybones"));
        }

        public void AddMarket(Market market)
        {
            string sql = "INSERT INTO markets (Name, Location, Description) VALUES (@Name, @Location, @Description)";
            _conn.Execute(sql, market);
        }

        public IEnumerable<Market> GetAllMarkets()
        {
            using var conn = new MySqlConnection(_conn.ConnectionString);

            string sql = "SELECT Id, Name, Description, Location, Image FROM Markets";
            return conn.Query<Market>(sql);
        }

        public Task<Market> GetMarketByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Market model)
        {
            using var conn = new MySqlConnection(_conn.ConnectionString);

            var sql = @"UPDATE Markets 
                        SET Name = @Name, 
                            Description = @Description, 
                            Location = @Location, 
                            Image = @Image 
                        WHERE Id = @Id";
            var affectedRows = conn.Execute(sql, model);
            return affectedRows > 0;
        }
    }
}
