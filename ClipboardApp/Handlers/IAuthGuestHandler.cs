using Microsoft.AspNetCore.Mvc;

namespace ClipboardApp.Handlers;

public class AuthGuestResponseDto
{
    public required string AuthToken { get; set; }
}

public interface IAuthGuestHandler
{
    Task<IActionResult> HandleAsync(string? sessionId);
}