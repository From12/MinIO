using Files.Application.Files.DTOs;
using MediatR;

namespace Files.Application.Files.Queries.GetFile;

public record GetFileByIdQuery : IRequest<FileDetailsDto?>
{
    public Guid FileId { get; init; }
    public bool GenerateDownloadUrl { get; init; } = true;
}
