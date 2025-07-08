namespace CityBonesPortfolio.Models
{
    public interface IMarketRepository
    {
        IEnumerable<Market> GetAllMarkets();
        void AddMarket(Market market);
    }
}
