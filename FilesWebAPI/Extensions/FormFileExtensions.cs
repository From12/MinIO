namespace FilesWebAPI.Extensions;

public static class FormFileExtensions
{
    private const long MaxFileSize = 100 * 1024 * 1024;

    public static bool IsValidSize(this IFormFile file)
    {
        return file.Length > 0 && file.Length <= MaxFileSize;
    }

    public static string GetExtension(this IFormFile file)
    {
        return Path.GetExtension(file.FileName).ToLowerInvariant();
    }

    public static bool HasAllowedExtension(this IFormFile file, IEnumerable<string> allowedExtensions)
    {
        var extension = file.GetExtension();
        return allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }
}