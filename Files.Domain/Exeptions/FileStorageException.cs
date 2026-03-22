namespace Files.Domain.Exceptions;


public class FileStorageException : DomainException
{
    public string Operation { get; }

    public FileStorageException(string operation, string message)
        : base($"Ошибка при выполнении операции '{operation}': {message}")
    {
        Operation = operation;
    }

    public FileStorageException(string operation, Exception innerException)
        : base($"Ошибка при выполнении операции '{operation}'.", innerException)
    {
        Operation = operation;
    }
    public FileStorageException(string operation, string message, Exception innerException)
    : base($"Ошибка при выполнении операции '{operation}': {message}", innerException)
    {
        Operation = operation;
    }
}