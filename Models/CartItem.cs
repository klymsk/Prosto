using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Prosto.Models;

public class CartItem
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Customer")]
    public int UserId { get; set; }

    [ForeignKey("Item")]
    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public Customer? Customer { get; set; }
    public Item? Item { get; set; }
}
