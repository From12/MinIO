using MediatR;

namespace Files.Application.Files.Queries.GetFile;

public record GetDownloadUrlQuery : IRequest<string>
{
    public Guid FileId { get; init; }
    public int ExpiryMinutes { get; init; } = 60;
}