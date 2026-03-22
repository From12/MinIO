using Files.Application.Common.Mappings;
using Files.Domain.Entities;
using AutoMapper;

namespace Files.Application.Files.DTOs;

public record FileDetailsDto : IMapWith<TFile>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string StoredName { get; init; } = string.Empty;
    public string BucketName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long Size { get; init; }
    public string? FileHash { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastAccessedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public bool IsDeleted { get; init; }
    public string? DownloadUrl { get; init; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TFile, FileDetailsDto>()
            .ForMember(dest => dest.DownloadUrl,
                opt => opt.Ignore());
    }
}