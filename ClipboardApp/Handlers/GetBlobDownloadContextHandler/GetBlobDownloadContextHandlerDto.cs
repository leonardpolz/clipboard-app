namespace ClipboardApp.Handlers.GetBlobDownloadContextHandler;
public class GetBlobDownloadContextHandlerDto
{
    public required string SasUri { get; init; }
    public required string OriginalFileName { get; init; } 
}
