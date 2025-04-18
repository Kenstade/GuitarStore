using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web.MinimalApi;
public static class Extensions
{
    public static IServiceCollection AddMinimalApiEndpoints(this IServiceCollection services, Assembly assembly)
    {
        foreach(var type in assembly.GetTypes()
            .Where(x => typeof(IEndpoint).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface))
        {
            services.AddTransient(typeof(IEndpoint), type);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        using var scope = builder.ServiceProvider.CreateScope();
        var endpoints = scope.ServiceProvider.GetServices<IEndpoint>();

        foreach(var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return builder;
    }
}
