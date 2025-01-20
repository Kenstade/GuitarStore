using System.Reflection;
using GuitarStore.Catalogs.Models;
using GuitarStore.Customers.Models;
using GuitarStore.Identity.Models;
using GuitarStore.Orders.Models;
using GuitarStore.ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Customers.Models.Address> Addresses { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
