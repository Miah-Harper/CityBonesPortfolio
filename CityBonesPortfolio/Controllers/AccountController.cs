using CityBonesPortfolio.Models;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using System.Configuration;
using System.Security.Claims;
namespace CityBonesPortfolio.Controllers
{
    public class AccountController : Controller
    {


        private readonly IConfiguration _config;

        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {

            if (ModelState.IsValid)
                return View(model);

            using var connection = new MySqlConnection(_config.GetConnectionString("citybones"));

            var user = connection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new {model.Email});

            if(user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PassWordHash))
            {
                ModelState.AddModelError("", "Invalid e-mail or password");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return user.IsAdmin ? RedirectToAction("Dashboard", "Admin") : RedirectToAction("Dasboard", "Account");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Register model)
        {
            if (!ModelState.IsValid) 
                return View(model);

            using var conn = new MySqlConnection(_config.GetConnectionString("citybones"));

            var existingUser = conn.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE Email = @Email",
                new { model.Email });

            if(existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(model);
            }

            string hashedPW = BCrypt.Net.BCrypt.HashPassword(model.Password);

            string? sql = "INSERT INTO Users (Email, PasswordHash, FullName, IsAdmin) " +
                "VALUES (@Email, @PasswordHash, @FullName, false);";

            conn.Execute(sql, new
            {
                model.Email,
                PasswordHash = hashedPW,
                model.FullName
            });

            return RedirectToAction("Login");
        }
    }
}
