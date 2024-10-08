using Azure.Storage.Sas;
using ClipboardApp.Shared.BinaryFileClient;

namespace ClipboardApp.Handlers.GetBlobDownloadContextHandler;

public class GetBlobDownloadContextHandler(
    IBinaryFileClient binaryFileClient,
    FileNameStorage fileNameStorage
) : IGetBlobDownloadContextHandler
{
    public async Task<GetBlobDownloadContextHandlerDto> HandleAsync(string sessionId)
    {
        var sasUri = await binaryFileClient.GenerateSasUri(sessionId, BlobSasPermissions.Read);

        return new GetBlobDownloadContextHandlerDto
        {
            SasUri = sasUri.ToString(),
            OriginalFileName = fileNameStorage.Storage[sessionId]
        };
    }
}