using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Prosto.Models;
using System.Security.Claims;

namespace Prosto.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly AppDbContext _context;

        public UserProfileController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Customers.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.ProductCount = _context.Orders.Count();
            return View(user); // <-- передаємо користувача у View
        }


        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(Customer model)
        {
            if (!ModelState.IsValid) return View(model);

            var exists = _context.Customers.Any(c => c.PhoneNumber == model.PhoneNumber || c.Email == model.Email && !string.IsNullOrEmpty(model.Email));
            if (exists)
            {
                ModelState.AddModelError("", "Користувач з таким номером або поштою вже існує");
                return View(model);
            }

            //// хешуємо пароль
            //model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            _context.Customers.Add(model);
            _context.SaveChanges();

            // зберігаємо у сесії
            HttpContext.Session.SetInt32("UserId", model.UserId);
            HttpContext.Session.SetString("FullName", model.FullName);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string phoneNumber, string password)
        {
            var user = _context.Customers.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            if (user != null && user.AuthProvider != "Google" && password == user.Password)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("FullName", user.FullName);
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Невірний номер або пароль");
            return View();
        }

        [HttpGet]
        public async Task LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "UserProfile");
            var props = new AuthenticationProperties { RedirectUri = redirectUrl };
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, props);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var externalResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var principal = externalResult?.Principal ?? result?.Principal;
            if (principal == null) return RedirectToAction("Login");

            var claims = result.Principal.Identities.First().Claims.ToList();

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                ?? claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                       ?? claims.FirstOrDefault(c => c.Type.Contains("name"))?.Value;

            var user = _context.Customers.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                user = new Customer
                {
                    Email = email,
                    FullName = name,
                    PhoneNumber = "",
                    AuthProvider = "Google" // Позначка, що користувач увійшов через Google
                };
                _context.Customers.Add(user);
                _context.SaveChanges();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Email", email);

            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
