using ClipboardApp.Shared.TextStorage;

namespace ClipboardApp.Handlers.SetTextClipboardHandler;

public class SetTextClipboardHandler(TextStorage textStorage) : ISetTextClipboardHandler
{
    private readonly Guid _clientSessionId = Guid.NewGuid();

    public async Task HandleAsync(SetTextClipboardHandlerRequestDto requestDto, string sessionId)
    {
        textStorage.UpdateStorage(sessionId, _clientSessionId, requestDto.Text);
    }
}