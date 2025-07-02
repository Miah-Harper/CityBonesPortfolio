using CityBonesPortfolio.Models;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using System.Configuration;
using System.Net;
using System.Net.Mail;
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {

            if (!ModelState.IsValid) //if model is valid go to login page
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
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return user.IsAdmin ? RedirectToAction("Dashboard", "Admin") : RedirectToAction("Dashboard", "Account");
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

        public IActionResult Dashboard()
        {
            return View();
        }

      

        public IActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required.");
                return View();
            }

            using var conn = new MySqlConnection(_config.GetConnectionString("citybones"));

            var user = conn.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE Email = @Email", new { Email = email });

            if (user != null)
            {
                // Here you'd generate a reset token and email it making it a secure token
               var token = Guid.NewGuid().ToString();
               var expiry = DateTime.UtcNow.AddMinutes(20);

                conn.Execute("UPDATE Users SET ResetToken = @Token, ResetTokenExpiry = @Expiry WHERE Email = @Email",
                   new {Token = token, Expiry = expiry, Email = email});

                //creating reset link
                var resetLink = Url.Action("ResetPassword", "Account",
                    new {token = token}, Request.Scheme);

                SendResetEmail(email, resetLink);
                
            }

            ViewBag.Message = "Password reset link has been sent!";
            return View();
        }

        private void SendResetEmail(string email, string resetLink)
        {
            var from = "calistaharper93@gmail.com";
            var password = "wvru hjcj ajff ospm"; //using app password

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(from, password),
                EnableSsl = true
            };

            var mail = new MailMessage(from, email)
            {
                Subject = "Reset Your Password",
                Body = $"Click the link to reset your password: {resetLink}",
                IsBodyHtml = false
            };

            smtpClient.Send(mail);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            return View(model: token); //passing token to view
        }

        [HttpPost]
        public IActionResult ResetPassword(string token, string newPassword)
        {
            using var conn = new MySqlConnection(_config.GetConnectionString("citybones"));

            var user = conn.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE ResetToken = @Token AND ResetTokenExpiry > NOW()",
                new {Token = token});

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid or expired link");
                return View(model: token);
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            //updating password
            conn.Execute("UPDATE Users SET PasswordHash = @Password, ResetToken = NULL, ResetTokenExpiry = NULL WHERE Id = @Id", 
                new {Password = newPassword, Id = user.Id});

            ViewBag.Message = "Password has successfully been reset!";
            return View();
        }
    }
}
