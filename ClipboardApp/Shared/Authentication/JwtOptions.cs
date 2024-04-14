namespace ClipboardApp.Shared.Authentication;

public class JwtOptions
{
    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public string? JwtSecret { get; set; }
}