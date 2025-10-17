using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Prosto.Models;

public class Order
{
    [Key]
    public int OrderId { get; set; }

    [ForeignKey("Customer")]
    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    [Precision(18, 2)]
    public decimal TotalPrice { get; set; }

    public string ShippingInfo { get; set; }

    public Customer Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}
