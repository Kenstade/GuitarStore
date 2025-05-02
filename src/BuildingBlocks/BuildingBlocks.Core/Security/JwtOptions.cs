namespace BuildingBlocks.Core.Security;

public sealed class JwtOptions
{
    public string Audience { get; set; } = null!;
    public string MetadataAddress { get; set; } = null!;
    public string[] ValidIssuer { get; set; } = null!;
}