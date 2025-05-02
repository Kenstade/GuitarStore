namespace BuildingBlocks.Core.Security;

public sealed class KeyCloakOptions
{
    public string AuthorizationUrl { get; set; } = null!;
    public string HealthUrl { get; set; } = null!;
    public string AdminUrl { get; set; } = null!;
    public string TokenUrl { get; set; } = null!;
    public string ConfidentialClientId { get; set; } = null!;
    public string ConfidentialClientSecret { get; set; } = null!;
    public string PublicClientId {get; set;} = null!;
}