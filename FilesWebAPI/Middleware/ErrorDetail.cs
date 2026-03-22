namespace FilesWebAPI.Middleware;

public class ErrorDetail
{
    public string? Property { get; set; }
    public string Message { get; set; } = string.Empty;
}