using Files.Domain.Entities;
using FluentAssertions;
using Xunit.v3;

namespace Files.Tests.Domain;

public class TFileTests
{
    [Fact]
    public void Create_ValidParameters_ShouldCreateFile()
    {
        var userId = Guid.NewGuid();
        var fileName = "test.pdf";
        var storedName = "guid-test.pdf";
        var bucketName = "files";
        var contentType = "application/pdf";
        var size = 1024L;

        var file = TFile.Create(userId, fileName, storedName, bucketName, contentType, size);

        file.Should().NotBeNull();
        file.Id.Should().NotBeEmpty();
        file.UserId.Should().Be(userId);
        file.FileName.Should().Be(fileName);
        file.StoredName.Should().Be(storedName);
        file.BucketName.Should().Be(bucketName);
        file.ContentType.Should().Be(contentType);
        file.Size.Should().Be(size);
        file.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        file.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyFileName_ShouldThrowArgumentException(string fileName)
    {
        Assert.Throws<ArgumentException>(() =>
            TFile.Create(Guid.NewGuid(), fileName, "stored.pdf", "bucket", "application/pdf", 1024));
    }

    [Fact]
    public void Create_FileNameTooLong_ShouldThrowArgumentException()
    {
        var longFileName = new string('a', 256);

        Assert.Throws<ArgumentException>(() =>
            TFile.Create(Guid.NewGuid(), longFileName, "stored.pdf", "bucket", "application/pdf", 1024));
    }

    [Fact]
    public void Create_ZeroSize_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            TFile.Create(Guid.NewGuid(), "test.pdf", "stored.pdf", "bucket", "application/pdf", 0));
    }

    [Fact]
    public void SoftDelete_ShouldMarkAsDeleted()
    {
        var file = TFile.Create(Guid.NewGuid(), "test.pdf", "stored.pdf", "bucket", "application/pdf", 1024);

        file.SoftDelete();

        file.IsDeleted.Should().BeTrue();
        file.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Restore_ShouldMarkAsNotDeleted()
    {
        var file = TFile.Create(Guid.NewGuid(), "test.pdf", "stored.pdf", "bucket", "application/pdf", 1024);
        file.SoftDelete();

        file.Restore();

        file.IsDeleted.Should().BeFalse();
        file.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void UpdateMetadata_ShouldUpdateFileNameAndContentType()
    {
        var file = TFile.Create(Guid.NewGuid(), "test.pdf", "stored.pdf", "bucket", "application/pdf", 1024);

        file.UpdateMetadata("renamed.pdf", "application/x-pdf");

        file.FileName.Should().Be("renamed.pdf");
        file.ContentType.Should().Be("application/x-pdf");
    }
}