using GuitarStore.Modules.ShoppingCarts.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.ShoppingCarts.Data;
public sealed class CartsDbContext(DbContextOptions<CartsDbContext> options) : DbContext(options)
{
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cart");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
