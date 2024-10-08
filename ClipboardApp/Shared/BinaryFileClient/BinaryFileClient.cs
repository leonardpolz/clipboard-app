using Azure.Core;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Uri = System.Uri;

namespace ClipboardApp.Shared.BinaryFileClient;

public class BinaryFileClient : IBinaryFileClient
{
    private BlobContainerClient? _containerClient;
    private BlobContainerClient? _sasContainerClient;
    private readonly int _sasLifeTimeInMinutes;
    private readonly BinaryFileClientOptions _clientOptions;

    public BinaryFileClient(IOptions<BinaryFileClientOptions> options)
    {
        _clientOptions = options.Value;
        _sasLifeTimeInMinutes = _clientOptions.SasLifeTimeInMinutes;

        _ = InitBlobClientAsync();
        _ = InitSasClient();
    }

    private async Task InitBlobClientAsync()
    {
        TokenCredential credential;

        if (string.IsNullOrEmpty(_clientOptions.ClientId) || string.IsNullOrEmpty(_clientOptions.ClientSecret))
        {
            credential = new ManagedIdentityCredential();
        }
        else
        {
            credential = new ClientSecretCredential(_clientOptions.TenantId, _clientOptions.ClientId,
                _clientOptions.ClientSecret);
        }

        var blobServiceClient = new BlobServiceClient(new Uri(_clientOptions.StorageAccountUrl), credential);
        _containerClient = blobServiceClient.GetBlobContainerClient(_clientOptions.ContainerName);
        await _containerClient.CreateIfNotExistsAsync();
    }

    private async Task InitSasClient()
    {
        var sasCredential =
            new StorageSharedKeyCredential(_clientOptions.StorageAccountName, _clientOptions.StorageAccountKey);
        var sasBlobServiceClient = new BlobServiceClient(new Uri(_clientOptions.StorageAccountUrl), sasCredential);
        _sasContainerClient = sasBlobServiceClient.GetBlobContainerClient(_clientOptions.ContainerName);
        await _sasContainerClient.CreateIfNotExistsAsync();
    }

    public async Task DeleteAsync(string fileName)
    {
        var blobClient = _containerClient!.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<Uri> GenerateSasUri(string fileName, BlobSasPermissions permission)
    {
        var blobClient = _sasContainerClient!.GetBlobClient(fileName);

        if (!blobClient.CanGenerateSasUri)
        {
            throw new InvalidOperationException("Blob client cannot generate SAS URI");
        }

        var expiresOn = DateTimeOffset.UtcNow.AddMinutes(this._sasLifeTimeInMinutes);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerClient!.Name,
            BlobName = fileName,
            Resource = "b",
            ExpiresOn = expiresOn
        };

        sasBuilder.SetPermissions(permission);

        return blobClient.GenerateSasUri(sasBuilder);
    }
}