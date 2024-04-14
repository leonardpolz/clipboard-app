using System.Collections;

namespace ClipboardApp.Shared.TextStorage;

public class TextStorage
{
    public Dictionary<string, string> Storage { get; } = new();
    public Dictionary<string, CancellationTokenSource> SessionCleanupCancellationTokens { get; set; } = new();
    
    public event Action<string, Guid>? StorageUpdated;

    public void UpdateStorage(string sessionId, Guid clientSessionId, string text)
    {
        Storage[sessionId] = text;
        StorageUpdated?.Invoke(sessionId, clientSessionId);
    }
}