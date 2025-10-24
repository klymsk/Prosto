using Microsoft.AspNetCore.Mvc;
using Prosto.Models;

namespace Prosto.Controllers
{
    public class ItemController : Controller
    {
        private readonly AppDbContext _context;

        public ItemController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            var item = _context.Items.FirstOrDefault(i => i.ItemId == id);
            if (item == null) return NotFound();

            return View(item);
        }
    }
}
