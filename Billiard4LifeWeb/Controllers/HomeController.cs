using Billiard4LifeWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Billiard4LifeWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["Active"] = "Home";
            return View();
        }

        public IActionResult Services()
        {
            ViewData["Active"] = "Services";
            return View();
        }

        public IActionResult Prices()
        {
            ViewData["Active"] = "Prices";
            return View();
        }

        public IActionResult PlaceOrder()
        {
            ViewData["Active"] = "PlaceOrder";
            return View();
        }

        public IActionResult Order()
        {
			TempData["SuccessMessage"] = "Thông tin đặt bàn của bạn đã được ghi nhận! Nhân viên sẽ sớm liên lạc với bạn để xác nhận!";

			return Redirect(Request.Headers["Referer"].ToString());
		}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}