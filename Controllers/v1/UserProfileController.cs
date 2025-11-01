using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prosto.Models;

namespace Prosto.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class UserProfileController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserProfileController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/v1/userprofile
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> Get()
    {
        return await _context.Customers.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> Get(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return NotFound();
        return customer;
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Post(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = customer.UserId }, customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Customer customer)
    {
        if (id != customer.UserId)
            return BadRequest();

        _context.Entry(customer).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return NotFound();

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
