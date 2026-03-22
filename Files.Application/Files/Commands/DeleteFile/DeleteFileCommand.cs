using MediatR;

namespace Files.Application.Files.Commands.DeleteFile;


public record DeleteFileCommand : IRequest<bool>
{
    public Guid FileId { get; init; }
    public Guid UserId { get; init; }
    public bool PermanentDelete { get; init; } = false;
}
