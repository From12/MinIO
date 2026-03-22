namespace Files.Application.Files.Commands.UploadFile;

public record UploadFileResult
{
    public Guid FileId { get; init; }

    public string FileName { get; init; } = string.Empty;

    public long Size { get; init; }

    public string ContentType { get; init; } = string.Empty;

    public string DownloadUrl { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }
}