using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prosto.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Prosto.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class CartItemController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartItemController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/v1/cartitem
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItem>>> Get()
    {
        return await _context.CartItems.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CartItem>> Get(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);

        if (cartItem == null) return NotFound();
        return cartItem;
    }

    [HttpPost]
    public async Task<ActionResult<CartItem>> Post(CartItem cartItem)
    {
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = cartItem.Id }, cartItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, CartItem cartItem)
    {
        if (id != cartItem.Id) return BadRequest();

        _context.Entry(cartItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);

        if (cartItem == null) return NotFound();

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}