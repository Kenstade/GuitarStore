using GuitarStore.Common.Extensions;
using GuitarStore.Data.Postgres;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Data.Extensions;

public static class EfCoreExtensions
{
    internal static IServiceCollection AddPostgresDbContext(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var postgresOptions = configuration.GetOptions<PostgresOptions>(nameof(PostgresOptions));
        if(postgresOptions == null) throw new ArgumentNullException(nameof(postgresOptions)); 

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
        return services;
    }
}
