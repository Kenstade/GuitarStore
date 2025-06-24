using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.ShoppingCarts.Models;

namespace MusicStore.Modules.ShoppingCarts.Data;
internal sealed class CartsDbContext(DbContextOptions<CartsDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "shopping_carts";
    
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
            
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
