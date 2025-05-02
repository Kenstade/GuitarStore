using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IdentityDbContext = GuitarStore.Modules.Identity.Data.IdentityDbContext;

namespace GuitarStore.Modules.Identity.Models;

internal sealed class User : Aggregate<Guid>
{   
    public string Email { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;

    public static User Create(Guid userId, string email, string password)
    {
        var user = new User
        {
            Id = userId,
            Email = email,
            Password = password
        };
        //user registered domain event
        return user;
    }
}

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", IdentityDbContext.DefaultSchema);
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email).HasMaxLength(50).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.Property(u => u.Password).HasMaxLength(50).IsRequired();
        
        builder.Property(u => u.PhoneNumber).HasMaxLength(15).IsRequired(false);
    }
}
