using Amazon;
using Amazon.S3;
using GuitarStore.Common.Core;
using GuitarStore.Data.Interfaces;
using GuitarStore.Data.Seed;

namespace GuitarStore.Common.S3Storage;

internal static class S3Extensions
{
    internal static IServiceCollection AddS3Configuration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3Options>(configuration.GetSection(nameof(S3Options)));

        var s3options = configuration.GetOptions<S3Options>(nameof(S3Options));
        if (s3options == null) throw new ArgumentNullException(nameof(s3options));

        var config = new AmazonS3Config
        {
            ServiceURL = s3options.ServiceUrl,
            AuthenticationRegion = s3options.Region,
            ForcePathStyle = s3options.ForcePathStyle,
        };

        services.AddSingleton<IAmazonS3>(new AmazonS3Client(s3options.AccessKey, s3options.SecretKey, config));
        services.AddSingleton<IS3Client, S3Client>();

        services.AddScoped<IDataSeeder, S3DataSeeder>();

        return services;
    }
}
