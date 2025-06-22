using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using GuitarStore.Modules.Identity.BackgroundJobs;
using GuitarStore.Modules.Identity.Data;
using GuitarStore.Modules.Identity.Extensions;
using GuitarStore.Modules.Identity.KeyCloak;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuitarStore.Modules.Identity;
public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<IdentityContext>(configuration);
        services.AddEventPublisher(typeof(IdentityModule).Assembly);
        
        services.AddScoped<ProcessOutboxMessagesJob>();

        services.AddKeyCloakIdentityProvider(configuration);
        
        services.AddTransient<IIdentityProvider, IdentityProvider>();
        
        services.AddPermissionAuthorization();
        services.AddQueryHandlers();
        
        services.AddValidatorsFromAssembly(typeof(IdentityModule).Assembly, includeInternalTypes: true);
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
