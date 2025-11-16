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
    
    [HttpGet("GoogleResponse")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        var principal = result?.Principal;
    
        if (principal == null)
            return Unauthorized(new { message = "Login failed" });
    
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var name = principal.FindFirstValue(ClaimTypes.Name);
    
        if (email == null)
            return BadRequest(new { message = "No email from Google" });
    
        var user = _context.Customers.FirstOrDefault(u => u.Email == email);
    
        if (user == null)
        {
            user = new Customer
            {
                Email = email,
                FullName = name,
                PhoneNumber = "",
                AuthProvider = "Google"
            };
    
            _context.Customers.Add(user);
            await _context.SaveChangesAsync();
        }
    
        var callbackUrl =
            $"http://localhost:5005/callback" +
            $"?userId={user.UserId}" +
            $"&fullName={Uri.EscapeDataString(user.FullName ?? "")}" +
            $"&email={Uri.EscapeDataString(user.Email ?? "")}";
    
        return Redirect(callbackUrl);
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
    
    [Route("blank")]
    public IActionResult Blank()
    {
        return Ok("Done");
    }
    
    [HttpGet("LastGoogleUser")]
    public async Task<IActionResult> LastGoogleUser()
    {
        var user = await _context.Customers
            .OrderByDescending(u => u.UserId)
            .FirstOrDefaultAsync();
    
        if (user == null)
            return NotFound();
    
        return Ok(user);
    }
}
