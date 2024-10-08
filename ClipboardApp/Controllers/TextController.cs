using System.IdentityModel.Tokens.Jwt;
using ClipboardApp.Handlers;
using ClipboardApp.Handlers.GetTextHandler;
using ClipboardApp.Handlers.GetWsTextHandler;
using ClipboardApp.Handlers.SetTextClipboardHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClipboardApp.Controllers;

[ApiController]
[Route("/api/v1/text")]
public class TextController(
    IGetTextHandler getTextHandler,
    ISetTextClipboardHandler setTextClipboardHandler,
    IWsGetTextHandler wsGetTextHandler
) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<GetTextHandlerDto> GetTextClipboard()
    {
        var sessionId = HttpContext.User.Claims.First(c => c.Type == "sessionId").Value;
        return await getTextHandler.HandleAsync(sessionId);
    }

    [Authorize]
    [HttpPatch]
    public async Task PatchTextClipboard(SetTextClipboardHandlerRequestDto requestDto)
    {
        var sessionId = HttpContext.User.Claims.First(c => c.Type == "sessionId").Value;
        await setTextClipboardHandler.HandleAsync(requestDto, sessionId);
    }

    [Route("/ws/text")]
    public async Task HandleWebSocketConnection(string token)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await HttpContext.Response.WriteAsync("Bad Request: The request is not a valid WebSocket request.");
            return;
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await HttpContext.Response.WriteAsync("Unauthorized: The token is invalid.");
            return;
        }

        var sessionId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sessionId")?.Value;

        if (sessionId == null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await HttpContext.Response.WriteAsync("Bad Request: The token does not contain a sessionId claim.");
            return;
        }

        var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await wsGetTextHandler.HandleAsync(webSocket, sessionId);
    }
}