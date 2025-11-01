using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prosto.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Prosto.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrderController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/v1/order
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> Get()
    {
        return await _context.Orders.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> Get(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null) return NotFound();
        return order;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Post(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = order.OrderId }, order);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Order order)
    {
        if (id != order.OrderId) return BadRequest();

        _context.Entry(order).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null) return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}