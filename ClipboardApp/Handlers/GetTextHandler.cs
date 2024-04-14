using AutoMapper;
using ClipboardApp.Data;
using ClipboardApp.Shared.TextStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClipboardApp.Handlers;

public class GetTextHandler : IGetTextHandler
{
    private readonly TextStorage _textStorage;
    
    public GetTextHandler(TextStorage textStorage)
    {
        _textStorage = textStorage;
    }
    
    public async Task<GetTextHandlerDto> HandleAsync(string sessionId)
    {
        return new GetTextHandlerDto()
        {
            Text = _textStorage.Storage[sessionId]
        };
    }
}