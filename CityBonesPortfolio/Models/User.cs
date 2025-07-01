namespace CityBonesPortfolio.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Email {  get; set; }

        public string? FullName { get; set; }
        public string? PassWordHash { get; set; }
        public bool IsAdmin { get; set; }
    }
}
