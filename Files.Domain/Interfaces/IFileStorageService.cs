namespace Files.Domain.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);
    Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string path,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadAsync(
        string storedName,
        CancellationToken cancellationToken = default);

    Task<string> GetPresignedUrlAsync(
        string storedName,
        int expiryMinutes = 60);

    Task<string> GetPresignedUploadUrlAsync(
        string fileName,
        string contentType,
        int expiryMinutes = 60);

    Task DeleteAsync(
        string storedName,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string storedName,
        CancellationToken cancellationToken = default);

    Task<FileMetadata> GetMetadataAsync(
        string storedName,
        CancellationToken cancellationToken = default);

    Task<StorageInfo> GetStorageInfoAsync();
}

public record FileMetadata(
    string FileName,
    string ContentType,
    long Size,
    DateTime LastModified,
    string? ETag);

public record StorageInfo(
    string BucketName,
    long TotalSize,
    long UsedSpace,
    int FilesCount);