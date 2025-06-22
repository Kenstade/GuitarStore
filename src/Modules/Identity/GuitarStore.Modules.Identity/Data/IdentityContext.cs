using BuildingBlocks.Core.Messaging.Outbox;
using GuitarStore.Modules.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Modules.Identity.Data;
internal sealed class IdentityContext(DbContextOptions<IdentityContext> options) : DbContext(options)
{
    public const string DefaultSchema = "identity";
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
