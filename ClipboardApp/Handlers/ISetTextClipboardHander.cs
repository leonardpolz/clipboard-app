namespace ClipboardApp.Handlers;

public class SetTextClipboardHandlerDto
{
    public string Text { get; set; } = string.Empty;
}

public interface ISetTextClipboardHandler
{ 
    Task HandleAsync(SetTextClipboardHandlerDto dto, string sessionId);
}