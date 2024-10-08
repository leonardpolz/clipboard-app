namespace ClipboardApp.Shared.Authentication;

public interface IJwtProvider
{
    string Generate(string sessionId, int lifetimeHours);
}