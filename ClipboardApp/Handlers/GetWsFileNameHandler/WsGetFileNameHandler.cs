using System.Net.WebSockets;
using System.Text;
using ClipboardApp.Shared.BinaryFileClient;

namespace ClipboardApp.Handlers.GetWsFileNameHandler;

public class WsGetFileNameHandler(
    FileNameStorage fileNameStorage
) : IWsGetFileNameHandler
{
    public async Task HandleAsync(WebSocket webSocket, string sessionId)
    {
        await SendTextAsync(webSocket, sessionId);
        if (!fileNameStorage.Storage.ContainsKey(sessionId)) await fileNameStorage.UpdateStorage(sessionId, "-");
    }

    private async Task SendTextAsync(WebSocket webSocket, string sessionId)
    {
        var tcs = new TaskCompletionSource<bool>();
        Action<string> handler = (updatedSessionId) =>
        {
            if (updatedSessionId == sessionId)
            {
                tcs.TrySetResult(true);
            }
        };

        fileNameStorage.StorageUpdated += handler;

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                tcs = new TaskCompletionSource<bool>();

                fileNameStorage.Storage.TryGetValue(sessionId, out var text);
                text ??= "-";

                var bytes = Encoding.UTF8.GetBytes(text);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text,
                    true, CancellationToken.None);

                await tcs.Task;
            }
        }
        finally
        {
            fileNameStorage.StorageUpdated -= handler;
        }
    }
}