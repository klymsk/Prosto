using Microsoft.AspNetCore.Mvc;
using Prosto.Models;
using System.Diagnostics;

namespace Prosto.Controllers
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
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }

        public IActionResult Category()
        {
            return View();
        }
    }
}
