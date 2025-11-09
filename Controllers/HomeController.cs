using Microsoft.AspNetCore.Mvc;
using Prosto.Models;
using System.Diagnostics;

namespace Prosto.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string category)
        {
            var items = string.IsNullOrEmpty(category)
            ? _context.Items.Take(16).ToList()
            : _context.Items.Where(i => i.Category == category).Take(16).ToList();

            return View(items);
        }

        public IActionResult Contacts()
        {
            return View();
        }

        public IActionResult Category(string category)
        {
            var items = string.IsNullOrEmpty(category)
            ? _context.Items.Take(30).ToList()
            : _context.Items.Where(i => i.Category == category).Take(30).ToList();

            ViewBag.CategoryName = category;

            return View(items);
        }
    }
}
