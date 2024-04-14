using System.Net.WebSockets;
using System.Text;
using ClipboardApp.Shared.BinaryFileClient;
using Microsoft.IdentityModel.Tokens;

namespace ClipboardApp.Handlers;

public class WsGetFileNameHandler : IWsGetFileNameHandler
{
    private readonly FileNameStorage _fileNameStorage;
    
    public WsGetFileNameHandler(FileNameStorage fileNameStorage)
    {
        _fileNameStorage = fileNameStorage;
    }

    public async Task HandleAsync(WebSocket webSocket, string sessionId)
    {
        await SendTextAsync(webSocket, sessionId);
        if(!_fileNameStorage.Storage.ContainsKey(sessionId)) 
            await _fileNameStorage.UpdateStorage(sessionId, "-");
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

        _fileNameStorage.StorageUpdated += handler;
        
        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                tcs = new TaskCompletionSource<bool>();

                _fileNameStorage.Storage.TryGetValue(sessionId, out var text);
                text ??= "-";
                
                var bytes = Encoding.UTF8.GetBytes(text);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                
                await tcs.Task;
            }
        }
        finally
        {
            _fileNameStorage.StorageUpdated -= handler;
        } 
    }
}