using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prosto.Models;

public class Seller
{
    [Key]
    public int SellerId { get; set; }

    public string SellerName { get; set; }

    public ICollection<Item>? Items { get; set; }
}
