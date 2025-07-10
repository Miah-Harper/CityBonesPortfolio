using CityBonesPortfolio.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace CityBonesPortfolio.Controllers
{
    public class ProductController : Controller
    {
        private readonly IConfiguration _config;

        public ProductController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
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

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageFileName = model.Image.FileName,
                ContentType = model.Image.ContentType,
                ImageData = memoryStream.ToArray()
            };

            using var connection = new MySqlConnection(_config.GetConnectionString("citybones"));

            string sql = @"INSERT INTO Product (Name, Description, Price, ImageFileName, ContentType, ImageData)
                VALUES (@Name, @Description, @Price, @ImageFileName, @ContentType, @ImageData);";

            await connection.ExecuteAsync(sql, product);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            using var conn = new MySqlConnection(_config.GetConnectionString("citybones"));
            var products = await conn.QueryAsync<Product>("SELECT * FROM Product");
            return View(products);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetImage(int id)
        {
            using var conn = new MySqlConnection(_config.GetConnectionString("citybones"));
            var product = await conn.QueryFirstOrDefaultAsync<Product>(
                "SELECT ContentType, ImageData FROM Product WHERE Id = @Id", 
                new {Id = id});

            if(product == null || product.ImageData == null)
                return NotFound();

            return File(product.ImageData, product.ContentType);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            using var conn = new MySqlConnection(_config.GetConnectionString("citybones"));
            var product = await conn.QueryFirstOrDefaultAsync<Product>("SELECT * FROM Product WHERE Id = @Id" ,
                new { Id = id});

            if(product == null)
                return NotFound();

            var model = new ProductViewModel
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
            };

            ViewBag.ProductId = id; // sending ID back in form
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var connection = new MySqlConnection(_config.GetConnectionString("citybones"));

            if (model.Image != null && model.Image.Length > 0)
            {
                using var ms = new MemoryStream();
                await model.Image.CopyToAsync(ms);

                string sql = @"UPDATE Product SET Name = @Name, Description = @Description, Price = @Price,
                ImageFileName = @ImageFileName, ContentType = @ContentType, ImageData = @ImageData WHERE Id = @Id";

                await connection.ExecuteAsync(sql, new
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    ImageFileName = model.Image.FileName,
                    ContentType = model.Image.ContentType,
                    ImageData = ms.ToArray(),
                    Id = id
                });
            }
            else
            {
                // Update without changing image
                string sql = @"UPDATE Products SET Name = @Name, Description = @Description, Price = @Price
                WHERE Id = @Id";

                await connection.ExecuteAsync(sql, new
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Id = id
                });
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = new MySqlConnection(_config.GetConnectionString("citybones"));
            await conn.ExecuteAsync("DELETE FROM Product WHERE Id = @Id",
                new { Id = id });
            return RedirectToAction("Index");
        }

    
    }
}
