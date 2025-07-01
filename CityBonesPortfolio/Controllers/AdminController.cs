using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Configuration;
using MySql.Data.MySqlClient;
using CityBonesPortfolio.Models;

namespace CityBonesPortfolio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DashBoard()
        {
            return View();
        }

        public IActionResult ManageUsers()
        {
            string conn = System.Configuration.ConfigurationManager.ConnectionStrings["citybones"].ConnectionString;

            using (var connection = new MySqlConnection(conn))
            {
                var users = connection.Query<User>("SELECT Id, Email FROM Users").ToList();
            }

            return View("Users");

        }

    }
}
