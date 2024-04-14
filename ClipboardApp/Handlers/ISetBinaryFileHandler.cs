namespace ClipboardApp.Handlers;

public class SetBinaryFileHandlerDto
{
    public byte[]? Data { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
}

public interface ISetBinaryFileHandler
{
    Task HandleAsync(SetBinaryFileHandlerDto dto, string sessionId);
}