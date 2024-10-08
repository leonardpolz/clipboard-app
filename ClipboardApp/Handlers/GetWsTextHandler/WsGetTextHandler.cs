using System.Net.WebSockets;
using System.Text;
using ClipboardApp.Shared.TextStorage;

namespace ClipboardApp.Handlers.GetWsTextHandler;

public class WsGetTextHandler(
    TextStorage textStorage
) : IWsGetTextHandler
{
    private readonly Guid _clientSessionId = Guid.NewGuid();

    public async Task HandleAsync(WebSocket webSocket, string sessionId)
    {
        _ = GetTextAsync(webSocket, sessionId);
        await SendTextAsync(webSocket, sessionId);
    }

    private async Task GetTextAsync(WebSocket webSocket, string sessionId)
    {
        var buffer = new byte[1024 * 64];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            textStorage.UpdateStorage(sessionId, _clientSessionId,
                System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count));
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    private async Task SendTextAsync(WebSocket webSocket, string sessionId)
    {
        var tcs = new TaskCompletionSource<bool>();
        Action<string, Guid> handler = (updatedSessionId, clientSessionId) =>
        {
            if (updatedSessionId == sessionId && clientSessionId != _clientSessionId)
            {
                tcs.TrySetResult(true);
            }
        };

        textStorage.StorageUpdated += handler;

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                tcs = new TaskCompletionSource<bool>();

                textStorage.Storage.TryGetValue(sessionId, out var text);
                text ??= "";

                var bytes = Encoding.UTF8.GetBytes(text);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text,
                    true, CancellationToken.None);

                await tcs.Task;
            }
        }
        finally
        {
            textStorage.StorageUpdated -= handler;
        }
    }
}