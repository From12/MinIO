namespace Files.Domain.Exceptions;

public class FileSizeExceededException : DomainException
{
    public long MaxSize { get; }
    public long ActualSize { get; }

    public FileSizeExceededException(long maxSize, long actualSize)
        : base($"Размер файла ({FormatSize(actualSize)}) превышает максимально допустимый ({FormatSize(maxSize)}).")
    {
        MaxSize = maxSize;
        ActualSize = actualSize;
    }

    private static string FormatSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int i = 0;
        double size = bytes;
        while (size >= 1024 && i < suffixes.Length - 1)
        {
            size /= 1024;
            i++;
        }
        return $"{size:0.##} {suffixes[i]}";
    }
}
