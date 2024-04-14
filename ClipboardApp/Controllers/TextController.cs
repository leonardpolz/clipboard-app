using System.IdentityModel.Tokens.Jwt;
using ClipboardApp.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClipboardApp.Controllers;

[ApiController]
[Route("/api/text")]
public class TextController : ControllerBase
{
    private readonly IGetTextHandler _getTextHandler;
    private readonly ISetTextClipboardHandler _setTextClipboardHandler;
    private readonly IWsGetTextHandler _wsGetTextHandler;
    
    public TextController(
        IGetTextHandler getTextHandler, 
        ISetTextClipboardHandler setTextClipboardHandler,
        IWsGetTextHandler wsGetTextHandler
    )
    {
        _getTextHandler = getTextHandler;
        _setTextClipboardHandler = setTextClipboardHandler;
        _wsGetTextHandler = wsGetTextHandler;
    }
    
    
    [Authorize]
    [HttpGet]
    public async Task<GetTextHandlerDto> GetTextClipboard()
    {
        var sessionId = HttpContext.User.Claims.First(c => c.Type == "sessionId").Value; 
        return await _getTextHandler.HandleAsync(sessionId);
    }
    
    [Authorize]
    [HttpPatch]
    public async Task PatchTextClipboard(SetTextClipboardHandlerDto dto)
    {
        var sessionId = HttpContext.User.Claims.First(c => c.Type == "sessionId").Value; 
        await _setTextClipboardHandler.HandleAsync(dto, sessionId);
    }
    
    [Route("/ws/text")]
    public async Task HandleWebSocketConnection(string token)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest) HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest; 
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
        
        if (jwtToken == null) HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        
        var sessionId = jwtToken.Claims.First(c => c.Type == "sessionId").Value;

        var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await _wsGetTextHandler.HandleAsync(webSocket, sessionId);
    }
}