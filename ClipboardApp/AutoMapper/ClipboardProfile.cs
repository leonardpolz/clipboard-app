using AutoMapper;
using ClipboardApp.Data.Models;
using ClipboardApp.Handlers;

namespace ClipboardApp.AutoMapper;

public class ClipboardProfile : Profile
{
    public ClipboardProfile()
    {
        CreateMap<ClipboardText, GetTextHandlerDto>();
        CreateMap<SetTextClipboardHandlerDto, ClipboardText>();
    } 
}