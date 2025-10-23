using Microsoft.AspNetCore.Mvc;

namespace Prosto.Controllers
{
    public class SellerProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
