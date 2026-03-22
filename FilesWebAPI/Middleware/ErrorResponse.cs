namespace FilesWebAPI.Middleware;

public class ErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<ErrorDetail>? Details { get; set; }
    public string? TraceId { get; set; }
}
