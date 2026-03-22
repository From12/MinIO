using Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Files.Persistence.Configurations;

/// <summary>
/// Конфигурация сущности TFile для Entity Framework Core
/// </summary>
public class TFileConfiguration : IEntityTypeConfiguration<TFile>
{
    public void Configure(EntityTypeBuilder<TFile> builder)
    {
        builder.ToTable("files");

        builder.HasKey(f => f.Id);

        // Свойства
        builder.Property(f => f.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(f => f.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(f => f.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(f => f.StoredName)
            .HasColumnName("stored_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(f => f.BucketName)
            .HasColumnName("bucket_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(f => f.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(f => f.Size)
            .HasColumnName("size")
            .IsRequired();

        builder.Property(f => f.FileHash)
            .HasColumnName("file_hash")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(f => f.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder.Property(f => f.LastAccessedAt)
            .HasColumnName("last_accessed_at")
            .IsRequired(false);

        builder.Property(f => f.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired(false);

        builder.Property(f => f.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(f => f.DeletedAt)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        // Индексы
        builder.HasIndex(f => f.Id)
            .IsUnique()
            .HasDatabaseName("ix_files_id");

        builder.HasIndex(f => f.UserId)
            .HasDatabaseName("ix_files_user_id");

        builder.HasIndex(f => f.StoredName)
            .IsUnique()
            .HasDatabaseName("ix_files_stored_name");

        builder.HasIndex(f => new { f.UserId, f.IsDeleted })
            .HasDatabaseName("ix_files_user_id_is_deleted");

        builder.HasIndex(f => f.ExpiresAt)
            .HasDatabaseName("ix_files_expires_at");

        builder.HasIndex(f => f.CreatedAt)
            .HasDatabaseName("ix_files_created_at");
    }
}