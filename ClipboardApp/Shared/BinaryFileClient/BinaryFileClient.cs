using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace ClipboardApp.Shared.BinaryFileClient;

public class BinaryFileClient: IBinaryFileClient
{
    private readonly BlobContainerClient _containerClient;

    public BinaryFileClient(IOptions<BinaryFileClientOptions> options)
    {
        var clientOptions = options.Value;
        TokenCredential credential;

        if (string.IsNullOrEmpty(clientOptions.ClientId) || string.IsNullOrEmpty(options.Value.ClientSecret))
        {
            credential = new ManagedIdentityCredential();
        }
        else
        {
            credential = new ClientSecretCredential(clientOptions.TenantId, clientOptions.ClientId, clientOptions.ClientSecret);
        }
        
        var blobServiceClient = new BlobServiceClient(new Uri(clientOptions.StorageAccountUrl), credential);
        _containerClient = blobServiceClient.GetBlobContainerClient(clientOptions.ContainerName);
        _containerClient.CreateIfNotExists();
    }
    
    public async Task UploadAsync(string fileName, byte[] data, string metaFileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(new MemoryStream(data), overwrite: true);
        var metadata = new Dictionary<string, string>
        {
            { "metaFileName", metaFileName }
        };
        await blobClient.SetMetadataAsync(metadata);
    }
 
    public async Task<BlobResult> DownloadAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        var response = await blobClient.DownloadAsync();
        await using var memoryStream = new MemoryStream();
        await response.Value.Content.CopyToAsync(memoryStream);

        var properties = await blobClient.GetPropertiesAsync();
        var metadata = properties.Value.Metadata;

        metadata.TryGetValue("metaFileName", out var metaFileName);

        return new BlobResult
        {
            FileName = metaFileName ?? string.Empty,
            Data = memoryStream.ToArray(),
            ContentType = properties.Value.ContentType ?? string.Empty
        };
    }
    
    public async Task DeleteAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }
}