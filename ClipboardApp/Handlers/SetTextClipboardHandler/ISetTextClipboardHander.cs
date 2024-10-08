namespace ClipboardApp.Handlers.SetTextClipboardHandler;

public interface ISetTextClipboardHandler
{
    Task HandleAsync(SetTextClipboardHandlerRequestDto requestDto, string sessionId);
}