namespace ClipboardApp.Handlers;

public class GetBinaryFileHandlerDto
{
    public byte[]? Data { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
}

public interface IGetBinaryFileHandler
{
    public Task<GetBinaryFileHandlerDto> HandleAsync(string sessionId);    
}