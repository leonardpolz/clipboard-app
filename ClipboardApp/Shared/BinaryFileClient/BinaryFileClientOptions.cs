namespace ClipboardApp.Shared.BinaryFileClient;

public class BinaryFileClientOptions
{
    public required string StorageAccountUrl { get; init; }
    public required string ContainerName { get; init; }
    public string? TenantId { get; init; }
    public string? ClientId { get; init; }
    public string? ClientSecret { get; set; }
    
    public required int SasLifeTimeInMinutes { get; init; }
    
    public required string StorageAccountName { get; init; }
    public required string? StorageAccountKey { get; set; }
}