using CityBonesPortfolio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CityBonesPortfolio.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppSettings _appSettings;

        public ContactController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Contact vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MailMessage msz = new MailMessage();
                    msz.From = new MailAddress(vm.Email);
                    msz.To.Add("calistaharper93@gmail.com");
                    msz.Subject = vm.Subject;
                    msz.Body = vm.Message;

                    SmtpClient smtp = new SmtpClient(); //simple mail transfer protocol
                    smtp.Host = _appSettings.SmtpHost;
                    smtp.Port = _appSettings.SmtpPort;
                    smtp.Credentials = new NetworkCredential(_appSettings.SmtpUsername, _appSettings.SmtpPassword);
                    smtp.EnableSsl = true;

                    smtp.Send(msz);

                    ModelState.Clear();
                    ViewBag.Message = "Thank you for contacting me! I will get back to your here shortly.";
                }
                catch (Exception ex)
                {
                    ModelState.Clear();
                    ViewBag.Message = $"Sorry we are facing Problem here {ex.Message}";
                }
            }

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
