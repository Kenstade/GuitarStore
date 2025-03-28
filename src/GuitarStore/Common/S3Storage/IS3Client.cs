namespace GuitarStore.Common.S3Storage;

public interface IS3Client
{
    Task PutObjectAsync(IFormFile file, string postfix);
    string GetImageUrl(string key);
    Task EnsureBucketExistsAsync();
}
