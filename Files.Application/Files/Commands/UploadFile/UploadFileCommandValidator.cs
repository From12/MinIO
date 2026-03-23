using FluentValidation;

namespace Files.Application.Files.Commands.UploadFile;


public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    // Максимальный размер файла - 100 MB
    private const long MaxFileSize = 100 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new()
    {
        // MIME стандарт
        // Изображения
        "image/jpg",
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "image/svg+xml",
        "image/bmp",

        // Документы
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "text/plain",
        "text/csv",

        // Архивы
        "application/zip",
        "application/x-rar-compressed",
        "application/x-7z-compressed",
        "application/gzip",

        // Видео
        "video/mp4",
        "video/quicktime",
        "video/x-msvideo",
        "video/webm",

        // Аудио
        "audio/mpeg",
        "audio/wav",
        "audio/ogg",
        "audio/webm"
    };

    public UploadFileCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("Имя файла обязательно")
            .MaximumLength(255).WithMessage("Имя файла не может превышать 255 символов")
            .Must(BeValidFileName).WithMessage("Имя файла содержит недопустимые символы");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content-Type обязателен")
            .Must(BeAllowedContentType).WithMessage(x =>
                $"Тип файла '{x.ContentType}' не поддерживается");

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("Файл не может быть пустым")
            .Must((command, stream) => BeValidSize(stream)).WithMessage(x =>
                $"Размер файла превышает максимально допустимый ({MaxFileSize / (1024 * 1024)} MB)");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Идентификатор пользователя обязателен");

        RuleFor(x => x.ExpiresAt)
            .Must(BeValidExpirationDate).WithMessage("Дата истечения должна быть в будущем")
            .When(x => x.ExpiresAt.HasValue && x.ExpiresAt.Value != default(DateTime));
    }

    private static bool BeValidFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        // Проверка на недопустимые символы
        var invalidChars = Path.GetInvalidFileNameChars();
        return !fileName.Any(c => invalidChars.Contains(c));
    }

    private static bool BeAllowedContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        var mainType = contentType.Split(';')[0].Trim().ToLowerInvariant();
        return AllowedContentTypes.Contains(mainType);
    }

    private static bool BeValidSize(Stream stream)
    {
        if (stream == null || stream == Stream.Null)
            return false;

        try
        {
            return stream.Length > 0 && stream.Length <= MaxFileSize;
        }
        catch
        {
            // Если невозможно определить длину потока, считаем валидным
            return true;
        }
    }

    private static bool BeValidExpirationDate(DateTime? expiresAt)
    {
        if (!expiresAt.HasValue)
            return true;

        // Игнорируем default(DateTime) = 0001-01-01
        if (expiresAt.Value == default(DateTime))
            return true;

        return expiresAt.Value > DateTime.UtcNow;
    }
}