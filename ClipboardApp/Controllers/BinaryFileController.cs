using System.IdentityModel.Tokens.Jwt;
using System.Net;
using ClipboardApp.Handlers;
using ClipboardApp.Handlers.GetBlobDownloadContextHandler;
using ClipboardApp.Handlers.GetBlobUploadContextHandler;
using ClipboardApp.Handlers.GetWsFileNameHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClipboardApp.Controllers;

[ApiController]
[Route("/api/v1/binary-file")]
public class BinaryFileController(
    IWsGetFileNameHandler wsGetFileNameHandler,
    IGetBlobDownloadContextHandler getBlobDownloadContextHandler,
    IGetBlobUploadContextHandler getBlobUploadContextHandler
) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [Route("blob-upload-context")]
    public async Task<GetBlobUploadContextHandlerDto> GetBlobUploadContext([FromQuery] string encodedFileName)
    {
        var decodedFileName = WebUtility.UrlDecode(encodedFileName);
        var sessionId = User.Claims.First(c => c.Type == "sessionId").Value;

        return await getBlobUploadContextHandler.HandleAsync(sessionId, decodedFileName);
    }

    [Authorize]
    [HttpGet]
    [Route("blob-download-context")]
    public async Task<GetBlobDownloadContextHandlerDto> GetBlobDownloadContext()
    {
        var sessionId = User.Claims.First(c => c.Type == "sessionId").Value;

        return await getBlobDownloadContextHandler.HandleAsync(sessionId);
    }

    [Route("/ws/file")]
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
        await wsGetFileNameHandler.HandleAsync(webSocket, sessionId);
    }
}