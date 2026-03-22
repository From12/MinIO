using Files.Application.Common.Mappings;
using Files.Domain.Entities;
using AutoMapper;

namespace Files.Application.Files.DTOs;


public record FileBriefDto : IMapWith<TFile>
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long Size { get; init; }
    public DateTime CreatedAt { get; init; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TFile, FileBriefDto>();
    }
}
