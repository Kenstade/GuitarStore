using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace BuildingBlocks.Core.Dapper;

public static class Extensions
{
    public static IServiceCollection AddDbConnectionFactory(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetOptions<PostgresOptions>(nameof(PostgresOptions)).ConnectionString;
        
        services.TryAddSingleton(new NpgsqlDataSourceBuilder(connectionString).Build());
        
        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();

        return services;
    }
}