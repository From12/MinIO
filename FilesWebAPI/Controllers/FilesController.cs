using Files.Application.Files.Commands.DeleteFile;
using Files.Application.Files.Commands.UploadFile;
using Files.Application.Files.DTOs;
using Files.Application.Files.Queries.GetFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FilesWebAPI.Controllers;


[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(UploadFileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UploadFileResult>> Upload(
        IFormFile file,
        [FromForm] Guid userId,
        [FromForm] DateTime? expiresAt = null)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "Файл не выбран или пуст" });
        }

        using var stream = file.OpenReadStream();

        var command = new UploadFileCommand
        {
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            UserId = userId,
            ExpiresAt = expiresAt
        };

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    // Получить информацию о файле по идентификатору
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FileDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FileDetailsDto>> GetById(Guid id)
    {
        var query = new GetFileByIdQuery { FileId = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound(new { message = $"Файл с идентификатором '{id}' не найден" });
        }

        return Ok(result);
    }

    // список файлов пользователя
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(FilesListVm), StatusCodes.Status200OK)]
    public async Task<ActionResult<FilesListVm>> GetByUser(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetFilesByUserQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    //URL для скачивания
    [HttpGet("{id:guid}/download-url")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GetDownloadUrl(
        Guid id,
        [FromQuery] int expiryMinutes = 60)
    {
        var query = new GetDownloadUrlQuery
        {
            FileId = id,
            ExpiryMinutes = expiryMinutes
        };

        try
        {
            var result = await _mediator.Send(query);
            return Ok(new { downloadUrl = result });
        }
        catch (Files.Domain.Exceptions.DomainFileNotFoundException)
        {
            return NotFound(new { message = $"Файл с идентификатором '{id}' не найден" });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromQuery] Guid userId,
        [FromQuery] bool permanent = false)
    {
        var command = new DeleteFileCommand
        {
            FileId = id,
            UserId = userId,
            PermanentDelete = permanent
        };

        await _mediator.Send(command);

        return NoContent();
    }
}