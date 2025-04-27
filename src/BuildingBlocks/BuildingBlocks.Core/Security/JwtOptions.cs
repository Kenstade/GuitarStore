namespace BuildingBlocks.Core.Security;

public sealed class JwtOptions
{
    public string Audience { get; set; } = default!;
    public string MetadataAddress { get; set; } = default!;
    public string[] ValidIssuer { get; set; } = default!;
}