using Files.Application.Files.Commands.UploadFile;
using Files.Application.Interfaces;
using Files.Domain.Entities;
using Files.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit.v3;

namespace Files.Tests.Application;

public class UploadFileCommandHandlerTests
{
    private readonly Mock<IFilesDbContext> _dbContextMock;
    private readonly Mock<IFileStorageService> _storageServiceMock;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<UploadFileCommandHandler>> _loggerMock;

    public UploadFileCommandHandlerTests()
    {
        _dbContextMock = new Mock<IFilesDbContext>();
        _storageServiceMock = new Mock<IFileStorageService>();
        _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<UploadFileCommandHandler>>();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnSuccessResult()
    {
        var fileStream = new MemoryStream("test content"u8.ToArray());
        var command = new UploadFileCommand
        {
            FileStream = fileStream,
            FileName = "test.pdf",
            ContentType = "application/pdf",
            UserId = Guid.NewGuid()
        };

        var filesDbSetMock = new Mock<Microsoft.EntityFrameworkCore.DbSet<TFile>>();
        _dbContextMock.Setup(x => x.Files).Returns(filesDbSetMock.Object);
        _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _storageServiceMock.Setup(x => x.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("stored-name.pdf");

        _storageServiceMock.Setup(x => x.GetPresignedUrlAsync(
                It.IsAny<string>(),
                It.IsAny<int>()))
            .ReturnsAsync("https://minio.example.com/presigned-url");

        var handler = new UploadFileCommandHandler(
            _dbContextMock.Object,
            _storageServiceMock.Object,
            _loggerMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.FileName.Should().Be("test.pdf");
        result.ContentType.Should().Be("application/pdf");
        result.DownloadUrl.Should().NotBeNullOrEmpty();

        _storageServiceMock.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}