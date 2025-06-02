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
        builder.ToTable("permissions")
            .HasKey(p => p.Code);

        builder.Property(p => p.Code)
            .HasMaxLength(100);
        
        var permissionCodes = new []
        {
            "carts:read", 
            "carts:update", 
            "carts:remove", 
            "orders:read", 
            "orders:create"
        };
        
        builder.HasData(permissionCodes.Select(permission => new Permission(permission)));
        
        builder.HasMany<Role>()
            .WithMany(r => r.Permissions)
            .UsingEntity(joinBuilder =>
            {
                joinBuilder.ToTable("role_permissions");
                joinBuilder.HasData(GetRolePermissions(permissionCodes));
            });
    }

    private static IEnumerable<object> GetRolePermissions(IEnumerable<string> permissionCodes)
    {
        var roles = new[] { Constants.Roles.User, Constants.Roles.Admin };
        
        return from role in roles
            from code in permissionCodes
            select new { RoleName = role, PermissionsCode = code };
    }
}