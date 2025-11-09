using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prosto.Models;

namespace Prosto.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "UserProfile");

            var cart = _context.CartItems
                .Include(c => c.Item)
                .Where(c => c.UserId == userId)
                .ToList();

            return View(cart);
        }

        public IActionResult AddToCart(int itemId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "UserProfile");

            var existing = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ItemId == itemId);

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = userId.Value,
                    ItemId = itemId,
                    Quantity = 1
                });
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int itemId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "UserProfile");

            var item = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ItemId == itemId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SaveOrder(string City, string Address, string Branch, string Payment)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "UserProfile");

            var cartItems = _context.CartItems
                .Include(c => c.Item)
                .Where(c => c.UserId == userId)
                .ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Кошик порожній.";
                return RedirectToAction("Index");
            }

            decimal totalPrice = cartItems.Sum(c => c.Item.Price * c.Quantity);

            string shippingInfo = $"{City}, {Address}, {Branch}, Оплата: {Payment}";

            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                ShippingInfo = shippingInfo
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var c in cartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.OrderId,
                    ItemId = c.ItemId,
                    Quantity = c.Quantity,
                    Price = c.Item.Price
                });
            }

            _context.CartItems.RemoveRange(cartItems);

            _context.SaveChanges();

            TempData["Success"] = "✅ Замовлення успішно оформлено!";
            return RedirectToAction("Index", "Home");
        }
    }
}
