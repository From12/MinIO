using Files.Application.Files.DTOs;

namespace Files.Application.Files.Queries.GetFile;

public record FilesListVm
{
    public IEnumerable<FileBriefDto> Files { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}
