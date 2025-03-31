using GuitarStore.Modules.ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.ShoppingCart.Data;
public sealed class CartDbContext(DbContextOptions<CartDbContext> options) : DbContext(options)
{
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cart");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
