namespace ClipboardApp.Handlers.GetBlobUploadContextHandler;

public interface IGetBlobUploadContextHandler
{
    Task<GetBlobUploadContextHandlerDto> HandleAsync(string sessionId, string fileName);
}
