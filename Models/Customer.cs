using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prosto.Models;

public class Customer
{
    [Key]
    public int UserId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? AuthProvider { get; set; } // Нове поле для типу аутентифікації

    public ICollection<CartItem>? CartItems { get; set; }
    public ICollection<Order>? Orders { get; set; }
}
