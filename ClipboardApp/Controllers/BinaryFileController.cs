using System.IdentityModel.Tokens.Jwt;
using ClipboardApp.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace ClipboardApp.Controllers;

[ApiController]
[Route("/api/binary-file")]
public class BinaryFileController : ControllerBase
{
   private readonly IGetBinaryFileHandler _getBinaryFileHandler;
   private readonly ISetBinaryFileHandler _setBinaryFileHandler;
   private readonly IWsGetFileNameHandler _wsGetFileNameHandler;

   public BinaryFileController(IGetBinaryFileHandler getBinaryFileHandler, ISetBinaryFileHandler setBinaryFileHandler, IWsGetFileNameHandler wsGetFileNameHandler)
   {
      _getBinaryFileHandler = getBinaryFileHandler;
      _setBinaryFileHandler = setBinaryFileHandler;
      _wsGetFileNameHandler = wsGetFileNameHandler;
   }
   
   [HttpGet]
   public async Task<IActionResult> GetBinaryFile()
   {
      var sessionId = User.Claims.FirstOrDefault(c => c.Type == "sessionId")?.Value;
      if (sessionId == null) return Unauthorized("SessionId not found");
      var file = await _getBinaryFileHandler.HandleAsync(sessionId);

      if (file.Data == null) return NotFound();

      return File(file.Data, file.ContentType, file.FileName);
   }

   [HttpPatch]
   public async Task<IActionResult> PatchBinaryFile([FromForm] IFormFile file)
   {
      using var memoryStream = new MemoryStream();
      await file.CopyToAsync(memoryStream);

      var dto = new SetBinaryFileHandlerDto()
      {
         Data = memoryStream.ToArray(),
         ContentType = file.ContentType,
         FileName = file.FileName
      };

      var sessionId = User.Claims.FirstOrDefault(c => c.Type == "sessionId")?.Value;
      if (sessionId == null) return Unauthorized("SessionId not found");
      await _setBinaryFileHandler.HandleAsync(dto, sessionId);

      return Ok(new { file.FileName });
   }
   
    [Route("/ws/file")]
    public async Task HandleWebSocketConnection(string token)
    {
       if (!HttpContext.WebSockets.IsWebSocketRequest)
       {
          HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
          return;
       }
       
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
        
        if (jwtToken == null) HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        
        var sessionId = jwtToken.Claims.First(c => c.Type == "sessionId").Value;

       var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
       await _wsGetFileNameHandler.HandleAsync(webSocket, sessionId);
    } 
}