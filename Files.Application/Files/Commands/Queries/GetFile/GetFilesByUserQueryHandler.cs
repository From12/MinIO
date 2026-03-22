using Files.Application.Files.DTOs;
using Files.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Files.Application.Files.Queries.GetFile;

public class GetFilesByUserQueryHandler : IRequestHandler<GetFilesByUserQuery, FilesListVm>
{
    private readonly IFilesDbContext _dbContext;

    public GetFilesByUserQueryHandler(IFilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FilesListVm> Handle(
        GetFilesByUserQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Files
            .AsNoTracking()
            .Where(f => f.UserId == request.UserId && !f.IsDeleted)
            .OrderByDescending(f => f.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var files = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FileBriefDto
            {
                Id = f.Id,
                FileName = f.FileName,
                ContentType = f.ContentType,
                Size = f.Size,
                CreatedAt = f.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new FilesListVm
        {
            Files = files,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}
