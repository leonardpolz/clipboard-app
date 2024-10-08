namespace ClipboardApp.Handlers.GetTextHandler;

public interface IGetTextHandler
{
    Task<GetTextHandlerDto> HandleAsync(string sessionId);
}