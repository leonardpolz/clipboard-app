namespace ClipboardApp.Handlers.GetBlobDownloadContextHandler;

public interface IGetBlobDownloadContextHandler
{
    Task<GetBlobDownloadContextHandlerDto> HandleAsync(string sessionId);
}
