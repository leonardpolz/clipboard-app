using ClipboardApp.Handlers;
using ClipboardApp.Handlers.AuthGuestHandler;
using Microsoft.AspNetCore.Mvc;

namespace ClipboardApp.Controllers;

[Route("api/v1/auth/guest")]
[ApiController]
public class AuthGuestController(IAuthGuestHandler authGuestHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AuthenticateAsync(string? sessionId)
    {
        return Ok(await authGuestHandler.HandleAsync(sessionId));
    }
}