using ClipboardApp.Shared.BinaryFileClient;

namespace ClipboardApp.Handlers;

public class GetBinaryFileHandler : IGetBinaryFileHandler
{
    private readonly IBinaryFileClient _binaryFileClient;
    
    public GetBinaryFileHandler(IBinaryFileClient binaryFileClient)
    {
        _binaryFileClient = binaryFileClient;
    }
    
    public async Task<GetBinaryFileHandlerDto> HandleAsync(string sessionId)
    {
        var blobResult = await _binaryFileClient.DownloadAsync(sessionId);
    
        return new GetBinaryFileHandlerDto() { Data = blobResult.Data, ContentType = blobResult.ContentType, FileName = blobResult.FileName };
    }
}