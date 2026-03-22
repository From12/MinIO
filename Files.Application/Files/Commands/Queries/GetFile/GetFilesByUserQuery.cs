using MediatR;

namespace Files.Application.Files.Queries.GetFile;


public record GetFilesByUserQuery : IRequest<FilesListVm>
{
    public Guid UserId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
