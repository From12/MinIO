namespace Files.Domain.Exceptions;

public class InvalidFileFormatException : DomainException
{
    public string? FileName { get; }
    public string? ContentType { get; }

    public InvalidFileFormatException(string message)
        : base(message) { }

    public InvalidFileFormatException(string fileName, string contentType)
        : base($"Неподдерживаемый формат файла '{fileName}' с Content-Type '{contentType}'.")
    {
        FileName = fileName;
        ContentType = contentType;
    }
}
