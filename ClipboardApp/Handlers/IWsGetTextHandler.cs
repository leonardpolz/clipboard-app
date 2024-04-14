using System.Net.WebSockets;

namespace ClipboardApp.Handlers;

public interface IWsGetTextHandler
{
    Task HandleAsync(WebSocket socket, string sessionId);
}