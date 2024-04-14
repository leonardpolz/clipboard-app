namespace ClipboardApp.Handlers;

public class GetTextHandlerDto
{
    public string Text { get; set; } = string.Empty;
}

public interface IGetTextHandler
{
    Task<GetTextHandlerDto> HandleAsync(string sessionId); 
}