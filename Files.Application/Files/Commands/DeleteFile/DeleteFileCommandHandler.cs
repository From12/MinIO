using Files.Application.Interfaces;
using Files.Domain.Exceptions;
using Files.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Files.Application.Files.Commands.DeleteFile;

public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, bool>
{
    private readonly IFilesDbContext _dbContext;
    private readonly IFileStorageService _storageService;
    private readonly ILogger<DeleteFileCommandHandler> _logger;

    public DeleteFileCommandHandler(
        IFilesDbContext dbContext,
        IFileStorageService storageService,
        ILogger<DeleteFileCommandHandler> logger)
    {
        _dbContext = dbContext;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<bool> Handle(
        DeleteFileCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Удаление файла: FileId={FileId}, UserId={UserId}, Permanent={Permanent}",
            request.FileId, request.UserId, request.PermanentDelete);

        var file = await _dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == request.FileId, cancellationToken);

        if (file == null)
        {
            _logger.LogWarning("Файл не найден: FileId={FileId}", request.FileId);
            throw new FileNotFoundException(request.FileId.ToString());
        }

        if (file.UserId != request.UserId)
        {
            _logger.LogWarning(
                "Попытка удаления чужого файла: FileId={FileId}, OwnerId={OwnerId}, RequestorId={RequestorId}",
                request.FileId, file.UserId, request.UserId);
            throw new UnauthorizedAccessException("У вас нет прав на удаление этого файла");
        }

        if (request.PermanentDelete)
        {
            try
            {
                await _storageService.DeleteAsync(file.StoredName, cancellationToken);
                _logger.LogInformation("Файл удален из хранилища: {StoredName}", file.StoredName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении файла из хранилища: {StoredName}", file.StoredName);
                // Продолжаем удаление записи из БД даже при ошибке хранилища
            }

            _dbContext.Files.Remove(file);
        }
        else
        {
            file.SoftDelete();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Файл успешно удален: FileId={FileId}", request.FileId);
        return true;
    }
}