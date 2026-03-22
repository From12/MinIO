using Files.Domain.Exceptions;
using Files.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace MinIOStorage
{
    public class MinioFileStorage : IFileStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioOptions _options;
        private readonly ILogger<MinioFileStorage> _logger;

        public MinioFileStorage(
            IMinioClient minioClient,
            IOptions<MinioOptions> options,
            ILogger<MinioFileStorage> logger)
        {
            _minioClient = minioClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<string> UploadAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            return await UploadAsync(fileStream, fileName, contentType, _options.DefaultBucket, cancellationToken);
        }

        public async Task<string> UploadAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            string bucketName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await EnsureBucketExistsAsync(bucketName, cancellationToken);

                // Сбрасываем позицию потока
                if (fileStream.CanSeek)
                {
                    fileStream.Position = 0;
                }

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType(contentType);

                await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

                _logger.LogInformation(
                    "Файл успешно загружен: Bucket={Bucket}, FileName={FileName}, Size={Size}",
                    bucketName, fileName, fileStream.Length);

                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке файла: {FileName}", fileName);
                throw new FileStorageException("upload", ex.Message, ex);
            }
        }

        public async Task<Stream> DownloadAsync(
            string storedName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var memoryStream = new MemoryStream();

                var getObjectArgs = new GetObjectArgs()
                    .WithBucket(_options.DefaultBucket)
                    .WithObject(storedName)
                    .WithCallbackStream((stream, ct) =>
                    {
                        stream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        return Task.CompletedTask;
                    });

                await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

                _logger.LogInformation("Файл скачан: {StoredName}", storedName);

                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при скачивании файла: {StoredName}", storedName);
                throw new FileStorageException("download", ex.Message, ex);
            }
        }

        public async Task<string> GetPresignedUrlAsync(
            string storedName,
            int expiryMinutes = 60)
        {
            try
            {
                var expiry = expiryMinutes > 0 ? expiryMinutes : _options.PresignedUrlExpiryMinutes;

                var presignedGetObjectArgs = new PresignedGetObjectArgs()
                    .WithBucket(_options.DefaultBucket)
                    .WithObject(storedName)
                    .WithExpiry(expiry);

                var url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);

                _logger.LogDebug(
                    "Presigned URL сгенерирован: {StoredName}, Expiry={Expiry}min",
                    storedName, expiry);

                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации presigned URL: {StoredName}", storedName);
                throw new FileStorageException("get_presigned_url", ex.Message, ex);
            }
        }

        public async Task<string> GetPresignedUploadUrlAsync(
            string fileName,
            string contentType,
            int expiryMinutes = 60)
        {
            try
            {
                var expiry = expiryMinutes > 0 ? expiryMinutes : _options.PresignedUrlExpiryMinutes;

                var presignedPutObjectArgs = new PresignedPutObjectArgs()
                    .WithBucket(_options.DefaultBucket)
                    .WithObject(fileName)
                    .WithExpiry(expiry);

                var url = await _minioClient.PresignedPutObjectAsync(presignedPutObjectArgs);

                _logger.LogDebug(
                    "Presigned upload URL сгенерирован: {FileName}, Expiry={Expiry}min",
                    fileName, expiry);

                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации presigned upload URL: {FileName}", fileName);
                throw new FileStorageException("get_presigned_upload_url", ex.Message, ex);
            }
        }

        public async Task DeleteAsync(
            string storedName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(_options.DefaultBucket)
                    .WithObject(storedName);

                await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

                _logger.LogInformation("Файл удален: {StoredName}", storedName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении файла: {StoredName}", storedName);
                throw new FileStorageException("delete", ex.Message, ex);
            }
        }

        public async Task<bool> ExistsAsync(
            string storedName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(_options.DefaultBucket)
                    .WithObject(storedName);

                await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
                return true;
            }
            catch (Minio.Exceptions.ObjectNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке существования файла: {StoredName}", storedName);
                throw new FileStorageException("exists", ex.Message, ex);
            }
        }

        public async Task<FileMetadata> GetMetadataAsync(
            string storedName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(_options.DefaultBucket)
                    .WithObject(storedName);

                var stat = await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);

                return new FileMetadata(
                    stat.ObjectName,
                    stat.ContentType,
                    stat.Size,
                    stat.LastModified,
                    stat.ETag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении метаданных файла: {StoredName}", storedName);
                throw new FileStorageException("get_metadata", ex.Message, ex);
            }
        }

        public async Task<StorageInfo> GetStorageInfoAsync()
        {
            try
            {
                return new StorageInfo(
                    _options.DefaultBucket,
                    0, // Общий размер неизвестен
                    0, // Использованное место неизвестно
                    0  // Количество файлов нужно считать отдельно
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении информации о хранилище");
                throw new FileStorageException("get_storage_info", ex.Message, ex);
            }
        }

        private async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken)
        {
            if (!_options.CreateBucketIfNotExists)
                return;

            try
            {
                var bucketExistsArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);

                var exists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

                if (!exists)
                {
                    var makeBucketArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);

                    await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);

                    _logger.LogInformation("Бакет создан: {BucketName}", bucketName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке/создании бакета: {BucketName}", bucketName);
                throw new FileStorageException("ensure_bucket", ex.Message, ex);
            }
        }
    }
}
