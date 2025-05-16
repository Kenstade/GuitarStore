using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Identity.Models;

internal sealed class Permission
{
    public Permission(string code)
    {
        Code = code;
    }
    public string Code { get; set; }
}

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        
        builder.HasKey(p => p.Code);

        builder.Property(p => p.Code)
            .HasMaxLength(100);
        
        builder.HasData(new Permission("carts:read"));
        
        builder.HasMany<Role>()
            .WithMany(r => r.Permissions)
            .UsingEntity(joinBuilder =>
            {
                joinBuilder.ToTable("role_permissions");
                joinBuilder.HasData(
                    CreateRolePermission(new Role("Customer"), new Permission("carts:read")),
                    CreateRolePermission(new Role("Admin"), new Permission("carts:read")));
            });
    }

    private static object CreateRolePermission(Role role, Permission permission)
    {
        return new
        {
            RoleName = role.Name,
            PermissionsCode = permission.Code,
        };
    }
}