using BuildingBlocks.Core.Messaging.Outbox;
using GuitarStore.Modules.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Identity.Data;
internal sealed class IdentityContext(DbContextOptions<IdentityContext> options) : DbContext(options)
{
    public const string DefaultSchema = "identity";
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
