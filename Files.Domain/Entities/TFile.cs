using System;
using System.Collections.Generic;
using System.Text;

namespace Files.Domain.Entities
{
    public class TFile
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string FileName { get; private set; } = string.Empty;

        public string StoredName { get; private set; } = string.Empty;

        public string BucketName { get; private set; } = string.Empty;

        public string ContentType { get; private set; } = string.Empty;

        public long Size { get; private set; }

        public string? FileHash { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? LastAccessedAt { get; private set; }

        public DateTime? ExpiresAt { get; private set; }

        public bool IsDeleted { get; private set; }

        public DateTime? DeletedAt { get; private set; }

        /* Конструктор для EF Core
        private TFile() { }
        */
        public static TFile Create(
            Guid userId,
            string fileName,
            string storedName,
            string bucketName,
            string contentType,
            long size,
            string? fileHash = null,
            DateTime? expiresAt = null)
        {
            ValidateFileName(fileName);
            ValidateContentType(contentType);
            ValidateSize(size);

            return new TFile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FileName = fileName,
                StoredName = storedName,
                BucketName = bucketName,
                ContentType = contentType,
                Size = size,
                FileHash = fileHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                IsDeleted = false
            };
        }

        public void MarkAsAccessed()
        {
            LastAccessedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                DeletedAt = DateTime.UtcNow;
            }
        }

        // Восстановление удаленного файла
        public void Restore()
        {
            if (IsDeleted)
            {
                IsDeleted = false;
                DeletedAt = null;
            }
        }

        public void UpdateMetadata(string fileName, string contentType)
        {
            ValidateFileName(fileName);
            ValidateContentType(contentType);

            FileName = fileName;
            ContentType = contentType;
        }

        private static void ValidateFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Имя файла не может быть пустым", nameof(fileName));

            if (fileName.Length > 255)
                throw new ArgumentException("Имя файла не может превышать 255 символов", nameof(fileName));
        }

        private static void ValidateContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content-Type не может быть пустым", nameof(contentType));
        }

        private static void ValidateSize(long size)
        {
            if (size <= 0)
                throw new ArgumentException("Размер файла должен быть больше нуля", nameof(size));

            // Максимальный размер файла - 5GB
            const long maxSize = 5L * 1024 * 1024 * 1024;
            if (size > maxSize)
                throw new ArgumentException($"Размер файла не может превышать {maxSize / (1024 * 1024 * 1024)} GB", nameof(size));
        }
    }
}
