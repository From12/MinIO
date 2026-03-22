using System.Net;
using System.Text.Json;
using Files.Domain.Exceptions;
using FluentValidation;

namespace FilesWebAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Code = "ValidationError";
                errorResponse.Message = "Ошибка валидации данных";
                errorResponse.Details = validationException.Errors
                    .Select(e => new ErrorDetail
                    {
                        Property = e.PropertyName,
                        Message = e.ErrorMessage
                    })
                    .ToList();
                break;

            case FileNotFoundException fileNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Code = "FileNotFound";
                errorResponse.Message = exception.Message;
                break;

            case InvalidFileFormatException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Code = "InvalidFileFormat";
                errorResponse.Message = exception.Message;
                break;

            case FileSizeExceededException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Code = "FileSizeExceeded";
                errorResponse.Message = exception.Message;
                break;

            case FileStorageException:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Code = "StorageError";
                errorResponse.Message = "Ошибка при работе с хранилищем файлов";
                _logger.LogError(exception, "Ошибка хранилища файлов");
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                errorResponse.Code = "AccessDenied";
                errorResponse.Message = "Недостаточно прав для выполнения операции";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Code = "InternalServerError";
                errorResponse.Message = "Произошла внутренняя ошибка сервера";
                _logger.LogError(exception, "Необработанное исключение");
                break;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
    }
}
