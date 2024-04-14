namespace ClipboardApp.Shared.BinaryFileClient;

public interface IBinaryFileClient
{
    Task UploadAsync(string fileName, byte[] data, string metaFileName);
    Task<BlobResult> DownloadAsync(string fileName);
    Task DeleteAsync(string fileName);
}

public class BlobResult
{
    public string FileName { get; init; } = string.Empty;
    public byte[] Data { get; init; }
    public string ContentType { get; init; } = string.Empty;
}

