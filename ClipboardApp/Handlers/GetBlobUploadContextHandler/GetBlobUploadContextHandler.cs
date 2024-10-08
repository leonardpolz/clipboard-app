using Azure.Storage.Sas;
using ClipboardApp.Shared.Authentication;
using ClipboardApp.Shared.BinaryFileClient;
using Microsoft.Extensions.Options;

namespace ClipboardApp.Handlers.GetBlobUploadContextHandler;

public class GetBlobUploadContextHandler(
    IBinaryFileClient binaryFileClient,
    FileNameStorage fileNameStorage,
    IOptions<JwtOptions> jwtOptions
) : IGetBlobUploadContextHandler
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    
    public async Task<GetBlobUploadContextHandlerDto> HandleAsync(string sessionId, string fileName)
    {
        await fileNameStorage.UpdateStorage(sessionId, fileName);
        _ = CleanUpBinaryFileAsync(sessionId);

        var sasUri = await binaryFileClient.GenerateSasUri(sessionId, BlobSasPermissions.Write);
        
        return new GetBlobUploadContextHandlerDto
        {
            SasUri = sasUri.ToString()
        };
    }

    private async Task CleanUpBinaryFileAsync(string sessionId)
    {
        await Task.Delay(TimeSpan.FromHours(_jwtOptions.AuthTokenLifetimeInHours));
        await binaryFileClient.DeleteAsync(sessionId);
        fileNameStorage.Storage.Remove(sessionId);
    }
}