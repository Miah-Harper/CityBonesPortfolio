using Dapper;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Data;

namespace CityBonesPortfolio.Models
{
    public class MarketRepository :IMarketRepository
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
            return _conn.Query<Market>("SELECT * FROM markets");
        }
    }
}
