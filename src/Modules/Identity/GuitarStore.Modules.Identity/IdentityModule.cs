using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.Identity.BackgroundJobs;
using GuitarStore.Modules.Identity.Data;
using GuitarStore.Modules.Identity.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarStore.Modules.Identity;
public static class IdentityModule
{
    public const string ModuleName = "Identity";
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<IdentityContext>(configuration);
        services.AddScoped<IDataSeeder, IdentityDataSeeder>();
        
        services.AddScoped<ProcessOutboxMessageJob>();
        
        services.AddValidatorsFromAssembly(typeof(IdentityModule).Assembly);
        services.AddMinimalApiEndpoints(typeof(IdentityModule).Assembly);
        
        return services;
    }

    public static IApplicationBuilder UseIdentityModule(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseMigration<IdentityContext>();
        app.UseBackgroundJobs(configuration);

        return app;
    }
}
