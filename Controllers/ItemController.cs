using Microsoft.AspNetCore.Mvc;

namespace Prosto.Controllers
{
    public class ItemController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
