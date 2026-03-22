using Files.Application.Interfaces;
using Files.Domain.Entities;
using Files.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Files.Application.Files.Commands.UploadFile;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, UploadFileResult>
{
    private readonly IFilesDbContext _dbContext;
    private readonly IFileStorageService _storageService;
    private readonly ILogger<UploadFileCommandHandler> _logger;

    public UploadFileCommandHandler(
        IFilesDbContext dbContext,
        IFileStorageService storageService,
        ILogger<UploadFileCommandHandler> logger)
    {
        _dbContext = dbContext;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<UploadFileResult> Handle(
        UploadFileCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Начало загрузки файла: {FileName}, ContentType: {ContentType}, UserId: {UserId}",
            request.FileName, request.ContentType, request.UserId);

        try
        {
            var fileExtension = Path.GetExtension(request.FileName);
            var storedName = $"{Guid.NewGuid()}{fileExtension}";

            await _storageService.UploadAsync(
                request.FileStream,
                storedName,
                request.ContentType,
                cancellationToken);

            _logger.LogInformation("Файл успешно загружен в хранилище: {StoredName}", storedName);

            long fileSize;
            try
            {
                fileSize = request.FileStream.Length;
            }
            catch
            {
                // Если поток не поддерживает Length, сбрасываем позицию
                request.FileStream.Position = 0;
                using var memoryStream = new MemoryStream();
                await request.FileStream.CopyToAsync(memoryStream, cancellationToken);
                fileSize = memoryStream.Length;
            }

            var file = TFile.Create(
                request.UserId,
                request.FileName,
                storedName,
                "files-bucket", // Можно вынести в конфигурацию
                request.ContentType,
                fileSize,
                expiresAt: request.ExpiresAt);

            await _dbContext.Files.AddAsync(file, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Информация о файле сохранена в БД: FileId={FileId}, StoredName={StoredName}",
                file.Id, file.StoredName);

            var downloadUrl = await _storageService.GetPresignedUrlAsync(storedName);

            return new UploadFileResult
            {
                FileId = file.Id,
                FileName = file.FileName,
                Size = file.Size,
                ContentType = file.ContentType,
                DownloadUrl = downloadUrl,
                CreatedAt = file.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла: {FileName}", request.FileName);
            throw;
        }
    }
}