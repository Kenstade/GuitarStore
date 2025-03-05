using GuitarStore.Common.Events;
using GuitarStore.Data.Interfaces;
using GuitarStore.Identity;
using GuitarStore.Identity.Events;
using GuitarStore.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace GuitarStore.Data.Seed;

internal sealed class IdentityDataSeeder(
    UserManager<User> userManager, 
    RoleManager<Role> roleManager, 
    INotifier notifier) : IDataSeeder
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;
    private readonly INotifier _notifier = notifier;

    public async Task SeedAllAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        if (!await _roleManager.RoleExistsAsync(Constants.Role.Admin))
        {
            await _roleManager.CreateAsync(new Role { Name = Constants.Role.Admin });
        }
        if (!await _roleManager.RoleExistsAsync(Constants.Role.User))
        {
            await _roleManager.CreateAsync(new Role { Name = Constants.Role.User });
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _userManager.FindByEmailAsync("user@test") == null)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "user@test",
                Email = "user@test"
            };

            var result = await _userManager.CreateAsync(user, "12345");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Constants.Role.User);
            }

            await _notifier.Send(new UserCreatedEvent(user.Id, user.Email));
        }

        if (await _userManager.FindByEmailAsync("admin@test") == null)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "admin@test",
                Email = "admin@test"
            };

            var result = await _userManager.CreateAsync(user, "54321");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Constants.Role.Admin);
            }

            await _notifier.Send(new UserCreatedEvent(user.Id, user.Email));
        }
    }
}
