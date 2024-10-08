namespace ClipboardApp.Shared.BinaryFileClient;

public class FileNameStorage
{
    public Dictionary<string, string> Storage { get; } = new();
    public event Action<string>? StorageUpdated;

    public async Task UpdateStorage(string sessionId, string text)
    {
        Storage[sessionId] = text;
        StorageUpdated?.Invoke(sessionId);
    }
}