using Azure.Storage.Sas;

namespace ClipboardApp.Shared.BinaryFileClient;

public interface IBinaryFileClient
{
    Task DeleteAsync(string fileName);
    Task<Uri> GenerateSasUri(string fileName, BlobSasPermissions permission);
}
