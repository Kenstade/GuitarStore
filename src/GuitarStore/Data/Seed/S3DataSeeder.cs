using GuitarStore.Common.S3Storage;
using GuitarStore.Data.Interfaces;

namespace GuitarStore.Data.Seed;

internal sealed class S3DataSeeder(IS3Client s3Client) : IDataSeeder
{
    private readonly IS3Client _s3Client = s3Client;
    public async Task SeedAllAsync()
    {
        await Task.CompletedTask;
        //await _s3Client.EnsureBucketExistsAsync();
        //TODO: await SeedImagesAsync();
    }
}
