using Microsoft.AspNetCore.Mvc;

namespace Prosto.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
