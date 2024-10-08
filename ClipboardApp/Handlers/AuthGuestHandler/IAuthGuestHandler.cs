namespace ClipboardApp.Handlers.AuthGuestHandler;

public interface IAuthGuestHandler
{
    Task<AuthGuestResponseDto> HandleAsync(string? sessionId);
}