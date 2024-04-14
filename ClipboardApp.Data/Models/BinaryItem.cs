namespace ClipboardApp.Data.Models;

public class BinaryItem
{
    public Guid Id { get; set; }
    public required byte[] Data { get; set; }
    public required string ContentType { get; set; }
    public required string FileName { get; set; }
}