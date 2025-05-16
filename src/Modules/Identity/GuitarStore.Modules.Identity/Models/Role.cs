using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Identity.Models;

internal sealed class Role : Entity
{
    internal Role(string name)
    {
        Name = name;
    }

    private readonly List<Permission> _permissions = [];
    
    public string Name { get; private set; }
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();
}

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Name);
        
        builder.Property(r => r.Name)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.HasData(new Role("Customer"), new Role("Admin"));

        builder.HasMany<User>()
            .WithMany(u => u.Roles)
            .UsingEntity(joinBuilder =>
            {
                joinBuilder.ToTable("user_roles");
                joinBuilder.Property("RolesName").HasColumnName("role_name"); // naming conventions package?
            });
        
    }
}