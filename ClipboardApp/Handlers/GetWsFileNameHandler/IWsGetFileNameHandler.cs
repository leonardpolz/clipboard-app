using System.Net.WebSockets;

namespace ClipboardApp.Handlers.GetWsFileNameHandler;

public interface IWsGetFileNameHandler
{
    Task HandleAsync(WebSocket webSocket, string sessionId);
}