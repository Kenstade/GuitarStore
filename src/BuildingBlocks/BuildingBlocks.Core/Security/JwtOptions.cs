namespace BuildingBlocks.Core.Security;

public sealed class JwtOptions
{
    public string Audience { get; init; } = null!;
    public string MetadataAddress { get; init; } = null!;
    public string[] ValidIssuer { get; init; } = null!;
}