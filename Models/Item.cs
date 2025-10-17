using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Prosto.Models;

public class Item
{
    [Key]
    public int ItemId { get; set; }

    [ForeignKey("Seller")]
    public int SellerId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
    public string Category { get; set; }

    [Precision(18, 2)]
    public decimal Price { get; set; }

    public Seller Seller { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}
