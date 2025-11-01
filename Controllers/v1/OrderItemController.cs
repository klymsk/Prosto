using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prosto.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Prosto.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class OrderItemController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrderItemController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/v1/orderitem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItem>>> Get()
    {
        return await _context.OrderItems.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderItem>> Get(int id)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null) return NotFound();
        return orderItem;
    }

    [HttpPost]
    public async Task<ActionResult<OrderItem>> Post(OrderItem orderItem)
    {
        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = orderItem.Id }, orderItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, OrderItem orderItem)
    {
        if (id != orderItem.Id) return BadRequest();

        _context.Entry(orderItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var orderItem = await _context.OrderItems.FindAsync(id);

        if (orderItem == null) return NotFound();

        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}