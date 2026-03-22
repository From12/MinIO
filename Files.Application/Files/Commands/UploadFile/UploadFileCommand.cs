using MediatR;

namespace Files.Application.Files.Commands.UploadFile;

public record UploadFileCommand : IRequest<UploadFileResult>
{
    public Stream FileStream { get; init; } = Stream.Null;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    // Опциональное время жизни файла
    public DateTime? ExpiresAt { get; init; }
}
