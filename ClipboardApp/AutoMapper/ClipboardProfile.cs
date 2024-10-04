using AutoMapper;
using ClipboardApp.Handlers;
using ClipboardApp.Models;

namespace ClipboardApp.AutoMapper;

public class ClipboardProfile : Profile
{
    public ClipboardProfile()
    {
        CreateMap<ClipboardText, GetTextHandlerDto>();
        CreateMap<SetTextClipboardHandlerDto, ClipboardText>();
    } 
}