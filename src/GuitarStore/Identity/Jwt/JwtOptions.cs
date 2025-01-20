namespace GuitarStore.Identity.Jwt;

public class JwtOptions
{
    public string SecretKey { get; set; } = null!;
    public int TokenLifeTimeMinutes { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}
