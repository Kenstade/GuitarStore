using BuildingBlocks.Core.EFCore.Interceptors;
using BuildingBlocks.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Core.EFCore;

public static class Extensions
{
    public static IServiceCollection AddPostgresDbContext<TContext>(this IServiceCollection services,
        IConfiguration configuration) where TContext : DbContext
    {
        services.Configure<PostgresOptions>(configuration.GetSection(nameof(PostgresOptions)));
        var postgresOptions = configuration.GetOptions<PostgresOptions>(nameof(PostgresOptions));

        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

        if (postgresOptions.UseInMemory)
        {
            services.AddDbContext<TContext>((sp,options) =>
            {
                options.UseInMemoryDatabase("guitarstoredb")
                .AddInterceptors(sp.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>());
            });
        }
        else
        {
            services.AddDbContext<TContext>((sp, options) =>
            {
                options.UseNpgsql(postgresOptions.ConnectionString)
                    .UseSnakeCaseNamingConvention()
                    .AddInterceptors(sp.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>());
            });
        }

        return services;
    }

    public static IApplicationBuilder UseMigration<TContext>(this IApplicationBuilder app) where TContext : DbContext
    {
        MigrateDatabaseAsync<TContext>(app.ApplicationServices).GetAwaiter().GetResult();

        SeedDataAsync(app.ApplicationServices).GetAwaiter().GetResult();
        
        return app;
    }

    private static async Task MigrateDatabaseAsync<TContext>(IServiceProvider sp) where TContext : DbContext
    {
        using var scope = sp.CreateScope();

        var postgresOptions = scope.ServiceProvider.GetRequiredService<IOptions<PostgresOptions>>();
        
        if (!postgresOptions.Value.UseInMemory)
        {
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            await context.Database.MigrateAsync();  
        }
    }

    private static async Task SeedDataAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();

        foreach (var seeder in seeders)
        {
            await seeder.SeedAllAsync();
        }
    }
}
