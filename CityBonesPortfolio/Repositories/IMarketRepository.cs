using CityBonesPortfolio.Models;

namespace CityBonesPortfolio.Repositories
{
    public interface IMarketRepository
    {
        IEnumerable<Market> GetAllMarkets();
        void AddMarket(Market market);

        Task<Market> GetMarketByIdAsync(int id);
        bool Update(Market model);
    }
}
