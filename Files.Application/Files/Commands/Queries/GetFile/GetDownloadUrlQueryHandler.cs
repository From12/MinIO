using Files.Application.Interfaces;
using Files.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Files.Application.Files.Queries.GetFile;


public class GetDownloadUrlQueryHandler : IRequestHandler<GetDownloadUrlQuery, string>
{
    private readonly IFilesDbContext _dbContext;
    private readonly IFileStorageService _storageService;

    public GetDownloadUrlQueryHandler(
        IFilesDbContext dbContext,
        IFileStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public async Task<string> Handle(
        GetDownloadUrlQuery request,
        CancellationToken cancellationToken)
    {
        var file = await _dbContext.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == request.FileId && !f.IsDeleted, cancellationToken);

        if (file == null)
            throw new FileNotFoundException(request.FileId.ToString());

        return await _storageService.GetPresignedUrlAsync(file.StoredName, request.ExpiryMinutes);
    }
}