﻿using BuildingBlocks.Core.EFCore.Interceptors;
using BuildingBlocks.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.EFCore;

public static class Extensions
{
    public static IServiceCollection AddPostgresDbContext<TContext>(this IServiceCollection services,
        IConfiguration configuration) where TContext : DbContext
    {
        var postgresOptions = configuration.GetOptions<PostgresOptions>(nameof(PostgresOptions));
        if (postgresOptions == null) throw new ArgumentNullException(nameof(postgresOptions));

        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

        if (postgresOptions.UseInMemory)
        {
            services.AddDbContext<TContext>((sp,options) =>
            {
                var interceptor = sp.GetService<ConvertDomainEventsToOutboxMessagesInterceptor>();
                options.UseInMemoryDatabase("guitarstoredb")
                .AddInterceptors(interceptor);
            });
        }
        else
        {
            services.AddDbContext<TContext>((sp, options) =>
            {
                var interceptor = sp.GetService<ConvertDomainEventsToOutboxMessagesInterceptor>();
                options.UseNpgsql(postgresOptions.ConnectionString)
                .AddInterceptors(interceptor);
            });
        }

        //services.AddIdentity<User, Role>(options =>
        //{
        //    options.Password.RequireDigit = false;
        //    options.Password.RequireLowercase = false;
        //    options.Password.RequireNonAlphanumeric = false;
        //    options.Password.RequireUppercase = false;
        //    options.Password.RequiredLength = 1;

        //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //    options.Lockout.MaxFailedAccessAttempts = 5;
        //    options.Lockout.AllowedForNewUsers = true;

        //    options.User.RequireUniqueEmail = true;
        //})
        //.AddEntityFrameworkStores<AppDbContext>()
        //.AddDefaultTokenProviders();

        return services;
    }

    public static IApplicationBuilder UseMigration<TContext>(this IApplicationBuilder app) where TContext : DbContext
    {
        //MigrateDatabaseAsync<TContext>(app.ApplicationServices).GetAwaiter().GetResult(); //TODO: проверка на inmemorydb

        SeedDataAsync(app.ApplicationServices).GetAwaiter().GetResult();
        
        return app;
    }

    private static async Task MigrateDatabaseAsync<TContext>(IServiceProvider sp) where TContext : DbContext
    {
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        await context.Database.MigrateAsync();       
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
