using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prosto.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Prosto.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class SellerController : ControllerBase
{
    private readonly AppDbContext _context;

    public SellerController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/v1/seller
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Seller>>> Get()
    {
        return await _context.Sellers.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Seller>> Get(int id)
    {
        var seller = await _context.Sellers.FindAsync(id);

        if (seller == null) return NotFound();
        return seller;
    }

    [HttpPost]
    public async Task<ActionResult<Seller>> Post(Seller seller)
    {
        _context.Sellers.Add(seller);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = seller.SellerId }, seller);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Seller seller)
    {
        if (id != seller.SellerId) return BadRequest();

        _context.Entry(seller).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var seller = await _context.Sellers.FindAsync(id);

        if (seller == null) return NotFound();

        _context.Sellers.Remove(seller);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}