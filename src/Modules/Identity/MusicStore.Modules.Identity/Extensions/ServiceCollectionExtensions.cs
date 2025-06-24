using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MusicStore.Modules.Identity.Authorization;
using MusicStore.Modules.Identity.Features;
using MusicStore.Modules.Identity.KeyCloak;

namespace MusicStore.Modules.Identity.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        
        return services;
    }

    public static IServiceCollection AddKeyCloakIdentityProvider(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<KeyCloakOptions>(configuration.GetSection(nameof(KeyCloakOptions)));
        
        services.AddTransient<KeyCloakAuthDelegatingHandler>();
        
        services.AddHttpClient<KeyCloakClient>((sp, httpClient) =>
        {
            var keyCloakOptions = sp.GetRequiredService<IOptions<KeyCloakOptions>>().Value;

            httpClient.BaseAddress = new Uri(keyCloakOptions.AdminUrl);
        }
        ).AddHttpMessageHandler<KeyCloakAuthDelegatingHandler>();
        
        return services;
    }

    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services
            .AddScoped<IQueryHandler<GetUserPermissions, GetUserPermissionsResponse>, GetUserPermissionsHandler>();

        return services;
    }
}