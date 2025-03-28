using GuitarStore.Common.Core;
using GuitarStore.Data;
using GuitarStore.Data.Interfaces;
using GuitarStore.Data.Seed;
using GuitarStore.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Common.EFCore;

internal static class EfCoreExtensions
{
    internal static IServiceCollection AddPostgresDbContextConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresOptions = configuration.GetOptions<PostgresOptions>(nameof(PostgresOptions));
        if (postgresOptions == null) throw new ArgumentNullException(nameof(postgresOptions));

        if (postgresOptions.UseInMemory)
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("guitarstoredb"));
        }
        else
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(postgresOptions.ConnectionString));
        }

        services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 1;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IDataSeeder, IdentityDataSeeder>();
        services.AddScoped<IDataSeeder, CatalogDataSeeder>();
        return services;
    }

    internal static async Task UseDataSeeders(this IApplicationBuilder app)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();
        var dataSeeders = scope.ServiceProvider.GetServices<IDataSeeder>();

        foreach (var seeder in dataSeeders)
        {
            await seeder.SeedAllAsync();
        }
    }
}
