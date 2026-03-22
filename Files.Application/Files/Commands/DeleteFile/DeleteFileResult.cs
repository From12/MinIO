namespace Files.Application.Files.Commands.DeleteFile;


public record DeleteFileResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}