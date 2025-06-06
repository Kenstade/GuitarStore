using BuildingBlocks.Core.Domain;
using GuitarStore.Modules.Identity.Events.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuitarStore.Modules.Identity.Models;

internal sealed class User : Aggregate<Guid>
{   
    private readonly List<Role> _roles = [];
     
    public string Email { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string IdentityId { get; private set; } = null!;
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    public static User Create(string email, string password, string identityId)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Password = password,
            IdentityId = identityId
        };

        user._roles.Add(new Role(Constants.Roles.User));
        
        user.AddDomainEvent(new UserRegistered(user.Id, user.Email));
        
        return user;
    }
}

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email).HasMaxLength(50).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.Property(u => u.Password).HasMaxLength(50).IsRequired();
        
        builder.Property(u => u.PhoneNumber).HasMaxLength(15).IsRequired(false);

        builder.HasIndex(u => u.IdentityId).IsUnique();
        builder.Property(u => u.IdentityId).HasMaxLength(50).IsRequired();
    }
}
