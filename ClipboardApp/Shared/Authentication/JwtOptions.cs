namespace ClipboardApp.Shared.Authentication;

public class JwtOptions
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public string? JwtSecret { get; set; }
    public required int AuthTokenLifetimeInHours { get; init; }
}