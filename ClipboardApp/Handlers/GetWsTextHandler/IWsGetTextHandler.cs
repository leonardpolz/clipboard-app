using System.Net.WebSockets;

namespace ClipboardApp.Handlers.GetWsTextHandler;

public interface IWsGetTextHandler
{
    Task HandleAsync(WebSocket socket, string sessionId);
}