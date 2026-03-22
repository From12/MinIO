using Files.Application.Files.DTOs;
using Files.Application.Interfaces;
using Files.Domain.Exceptions;
using Files.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Files.Application.Files.Queries.GetFile;


public class GetFileByIdQueryHandler : IRequestHandler<GetFileByIdQuery, FileDetailsDto?>
{
    private readonly IFilesDbContext _dbContext;
    private readonly IFileStorageService _storageService;

    public GetFileByIdQueryHandler(
        IFilesDbContext dbContext,
        IFileStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public async Task<FileDetailsDto?> Handle(
        GetFileByIdQuery request,
        CancellationToken cancellationToken)
    {
        var file = await _dbContext.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == request.FileId && !f.IsDeleted, cancellationToken);

        if (file == null)
            return null;

        string? downloadUrl = null;
        if (request.GenerateDownloadUrl)
        {
            downloadUrl = await _storageService.GetPresignedUrlAsync(file.StoredName);
        }

        return new FileDetailsDto
        {
            Id = file.Id,
            UserId = file.UserId,
            FileName = file.FileName,
            StoredName = file.StoredName,
            BucketName = file.BucketName,
            ContentType = file.ContentType,
            Size = file.Size,
            FileHash = file.FileHash,
            CreatedAt = file.CreatedAt,
            LastAccessedAt = file.LastAccessedAt,
            ExpiresAt = file.ExpiresAt,
            IsDeleted = file.IsDeleted,
            DownloadUrl = downloadUrl
        };
    }
}
