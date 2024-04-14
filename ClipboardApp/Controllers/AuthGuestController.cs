using ClipboardApp.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace ClipboardApp.Controllers;

[Route("api/auth/guest")]
[ApiController]
public class AuthGuestController : ControllerBase
{
    private readonly IAuthGuestHandler _authGuestHandler;
    
    public AuthGuestController(IAuthGuestHandler authGuestHandler)
    {
        _authGuestHandler = authGuestHandler;
    }

    [HttpPost]
    public async Task<IActionResult> AuthenticateAsync(string? sessionId)
    {
        return await _authGuestHandler.HandleAsync(sessionId);
    }
}