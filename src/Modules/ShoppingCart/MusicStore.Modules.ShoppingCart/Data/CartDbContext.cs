using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.ShoppingCart.Models;

namespace MusicStore.Modules.ShoppingCart.Data;
internal sealed class CartDbContext(DbContextOptions<CartDbContext> options) : DbContext(options)
{
    public const string DefaultSchema = "shopping_cart";
    
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
            
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
