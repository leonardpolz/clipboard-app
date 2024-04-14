using ClipboardApp.Data;
using ClipboardApp.Data.Models;
using ClipboardApp.Shared.BinaryFileClient;
using Microsoft.EntityFrameworkCore;

namespace ClipboardApp.Handlers;

public class SetBinaryFileHandler : ISetBinaryFileHandler
{
    private readonly IBinaryFileClient _binaryFileClient;
    private readonly FileNameStorage _fileNameStorage;
    private const int BinaryFileLifeTimeInHours = 8;

    public SetBinaryFileHandler(IBinaryFileClient binaryFileClient, FileNameStorage fileNameStorage)
    {
        _binaryFileClient = binaryFileClient;
        _fileNameStorage = fileNameStorage;
    }
    
    public async Task HandleAsync(SetBinaryFileHandlerDto dto, string sessionId)
    {
        if (dto.Data == null) return;
        await _binaryFileClient.UploadAsync(sessionId, dto.Data, dto.FileName);
        await _fileNameStorage.UpdateStorage(sessionId, dto.FileName);
        _ = CleanUpBinaryFileAsync(sessionId);
    }
    
    private async Task CleanUpBinaryFileAsync(string sessionId)
    {
        await Task.Delay(TimeSpan.FromHours(BinaryFileLifeTimeInHours));
        await _binaryFileClient.DeleteAsync(sessionId);
        _fileNameStorage.Storage.Remove(sessionId);
    }
}