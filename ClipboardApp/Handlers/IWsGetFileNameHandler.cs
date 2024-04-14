using System.Net.WebSockets;

namespace ClipboardApp.Handlers;

public interface IWsGetFileNameHandler
{
    Task HandleAsync(WebSocket webSocket, string sessionId);
}