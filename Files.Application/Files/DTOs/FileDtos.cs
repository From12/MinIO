using Files.Application.Common.Mappings;
using Files.Domain.Entities;
using AutoMapper;

namespace Files.Application.Files.DTOs;

public record FileDto : IMapWith<TFile>
{
    public Guid Id { get; init; }

    public string FileName { get; init; } = string.Empty;

    public string ContentType { get; init; } = string.Empty;

    public long Size { get; init; }

    public string SizeFormatted { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public string? DownloadUrl { get; init; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TFile, FileDto>()
            .ForMember(dest => dest.SizeFormatted,
                opt => opt.MapFrom(src => FormatSize(src.Size)))
            .ForMember(dest => dest.DownloadUrl,
                opt => opt.Ignore()); 
    }

    private static string FormatSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int i = 0;
        double size = bytes;
        while (size >= 1024 && i < suffixes.Length - 1)
        {
            size /= 1024;
            i++;
        }
        return $"{size:0.##} {suffixes[i]}";
    }
}
