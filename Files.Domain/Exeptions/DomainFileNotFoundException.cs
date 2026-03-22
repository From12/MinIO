namespace Files.Domain.Exceptions;

public class DomainFileNotFoundException : DomainException
{
    public Guid FileId { get; }

    public DomainFileNotFoundException(Guid fileId)
        : base($"Файл с идентификатором '{fileId}' не найден.")
    {
        FileId = fileId;
    }

    public DomainFileNotFoundException(string storedName)
        : base($"Файл '{storedName}' не найден в хранилище.")
    {
        FileId = Guid.Empty;
    }
}
