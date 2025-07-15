using CityBonesPortfolio.Models;
using CityBonesPortfolio.Repositories;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;


namespace CityBonesPortfolio.Controllers
{
    public class MarketsController : Controller
    {
        private readonly IMarketRepository _repo;
        private readonly IConfiguration _config;

        public MarketsController(IMarketRepository repo, IConfiguration configuration)
        {
            _repo = repo;
            _config = configuration;
        }

        public IActionResult Index()
        {
            var markets = _repo.GetAllMarkets();

            var viewModels = markets.Select(x => new MarketViewModel
            {
                Name = x.Name,
                Description = x.Description,
                Location = x.Location,
                ImageData = x.Image // Assuming Image is a byte array in the Market model
            }).ToList();
            return View(viewModels);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MarketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.Image == null || model.Image.Length == 0)
            {
                ModelState.AddModelError("Image", "Please upload an image.");
                return View(model);
            }
            using var memoryStream = new MemoryStream();
            await model.Image.CopyToAsync(memoryStream);
            // Create a new Market object and populate it with the data from the model
            var market = new Market
            {
                Name = model.Name,
                Description = model.Description,
                Location = model.Location,
                Image = memoryStream.ToArray()

            };

            using var conn = new MySqlConnection(_config.GetConnectionString("citybones"));
            string sql = @"INSERT INTO Markets (Name, Description, Location, Image)
                VALUES (@Name, @Description, @Location, @Image);";


            await conn.ExecuteAsync(sql, market);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var market = await _repo.GetMarketByIdAsync(id);
            if (market == null)
            {
                return NotFound();
            }

            var model = new MarketViewModel
            {
                Id = market.Id,
                Name = market.Name,
                Description = market.Description,
                Location = market.Location,
                HasExistingImage = market.Image != null // Assuming Image is a byte array in the Market model
            };
            return View(market);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MarketViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest("Market ID mismatch.");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            byte[]? imageData = null;
            string? contentType = null;

            if (model.Image != null && model.Image.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                model.Image.CopyTo(memoryStream);
                imageData = memoryStream.ToArray();
                contentType = model.Image.ContentType;
            }

            var market = new Market
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Location = model.Location,
                Image = imageData,
                ContentType = contentType
            };

            var updated = _repo.Update(market);
            if (updated)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "An error occurred while updating the market.");
                return View(model);

            }

            return RedirectToAction(nameof(Index));
        }
    }
}
