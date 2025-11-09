using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace Prosto.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
    { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CartItem>()
            .HasOne(c => c.Customer)
            .WithMany(u => u.CartItems)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(c => c.Item)
            .WithMany(i => i.CartItems)
            .HasForeignKey(c => c.ItemId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(o => o.Order)
            .WithMany(or => or.OrderItems)
            .HasForeignKey(o => o.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(o => o.Item)
            .WithMany(i => i.OrderItems)
            .HasForeignKey(o => o.ItemId);
    }
}
