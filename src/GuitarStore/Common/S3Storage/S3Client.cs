using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace GuitarStore.Common.S3Storage;
// TODO: доделать
internal sealed class S3Client(IAmazonS3 s3, IOptions<S3Options> s3Options, ILogger<S3Client> logger) : IS3Client
{
    private readonly IAmazonS3 _s3 = s3;
    private readonly S3Options _s3Options = s3Options.Value;
    private readonly ILogger<S3Client> _logger = logger;

    public async Task PutObjectAsync(IFormFile file, string postfix)
    {
        using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = _s3Options.BucketName,
            Key = postfix + "/",
            InputStream = stream,
            ContentType = file.ContentType,
            Metadata =
            {
                ["file-name"] = file.FileName
            }
        };
        await _s3.PutObjectAsync(request);
    } 

    public string GetImageUrl(string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _s3Options.BucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(20)
        };

        return _s3.GetPreSignedURL(request);
    }
    public async Task EnsureBucketExistsAsync()
    {
        await _s3.EnsureBucketExistsAsync(_s3Options.BucketName);
    }

}
