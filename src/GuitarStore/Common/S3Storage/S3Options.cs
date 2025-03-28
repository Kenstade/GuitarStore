namespace GuitarStore.Common.S3Storage;

internal sealed class S3Options
{
    public string ServiceUrl { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string BucketName { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public bool ForcePathStyle { get; init; }
}
