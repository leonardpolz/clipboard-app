using ClipboardApp.Shared.TextStorage;

namespace ClipboardApp.Handlers.GetTextHandler;

public class GetTextHandler(
    TextStorage textStorage
) : IGetTextHandler
{
    public async Task<GetTextHandlerDto> HandleAsync(string sessionId)
    {
        return new GetTextHandlerDto
        {
            Text = textStorage.Storage[sessionId]
        };
    }
}