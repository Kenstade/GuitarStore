namespace BuildingBlocks.Core.Security;

public sealed class KeyCloakOptions
{
    public string AuthorizationUrl { get; init; } = null!;
    public string HealthUrl { get; init; } = null!;
    public string AdminUrl { get; init; } = null!;
    public string TokenUrl { get; init; } = null!;
    public string ConfidentialClientId { get; init; } = null!;
    public string ConfidentialClientSecret { get; init; } = null!;
    public string PublicClientId {get; init;} = null!;
}