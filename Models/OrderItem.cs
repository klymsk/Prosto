using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Prosto.Models;

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }

    [ForeignKey("Item")]
    public int ItemId { get; set; }

    public int Quantity { get; set; }

    [Precision(18, 2)]
    public decimal Price { get; set; }

    public Order Order { get; set; }
    public Item Item { get; set; }
}
