using CityBonesPortfolio.Models;
using CityBonesPortfolio.Repositories;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace CityBonesPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly string _connString;
        private readonly IUserProductRepository _userProductRepository;

        //combing all dependencies in one constructor
        
        public HomeController(
                 ILogger<HomeController> logger,
                 IConfiguration config,
                 IProductRepository productRepository,
                 IUserProductRepository userProductRepository)
        {
            _logger = logger;
            _connString = config.GetConnectionString("citybones");
            _productRepository = productRepository;
            _userProductRepository = userProductRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _userProductRepository.GetAllProductsAsync();
            return View(products);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetImage(int id)
        {
            using var conn = new MySqlConnection(_connString);

            string sql = "SELECT ImageData FROM Product WHERE Id = @Id";

            var imageBytes = await conn.ExecuteScalarAsync<byte[]>(sql, new { Id = id });

            if(imageBytes != null)
            {
                return File(imageBytes, "img/jpeg");
            }

            return NotFound();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}