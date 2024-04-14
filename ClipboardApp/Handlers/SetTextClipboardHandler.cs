using AutoMapper;
using ClipboardApp.Data;
using ClipboardApp.Data.Models;
using ClipboardApp.Shared.TextStorage;
using Microsoft.EntityFrameworkCore;

namespace ClipboardApp.Handlers;

public class SetTextClipboardHandler : ISetTextClipboardHandler
{
    private readonly TextStorage _textStorage;
    private readonly Guid _clientSessionId = Guid.NewGuid();
    
    public SetTextClipboardHandler(TextStorage textStorage)
    {
       _textStorage = textStorage; 
    }
    
    public async Task HandleAsync(SetTextClipboardHandlerDto dto, string sessionId)
    {
        _textStorage.UpdateStorage(sessionId, _clientSessionId, dto.Text);
    }
}