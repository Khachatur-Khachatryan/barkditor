using System.IO;
using System.Threading.Tasks;
using Barkditor.Protobuf;
using BarkditorServer.BusinessLogic.Services;
using BarkditorServer.Domain.Constants;
using BarkditorServer.UnitTests.Helpers;
using FluentAssertions;
using Grpc.Core;
using Moq;
using Xunit;

namespace BarkditorServer.UnitTests.Tests.FilesServiceTests;

[Collection("Sequential")]
public class RemoveFileOrDirectoryTests
{
    [Fact]
    public async Task RemoveFileTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var contextMoq = new Mock<ServerCallContext>();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "newFile.txt");
        var fileCreateRequest = new CreateFileOrDirectoryRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        await service.CreateFileOrDirectory(fileCreateRequest, contextMoq.Object);
        var request = new RemoveFileOrDirectoryRequest
        {
            Path = filePath,
            IsDirectory = false
        };

        await service.RemoveFileOrDirectory(request, contextMoq.Object);

        var fileExists = File.Exists(filePath);
        fileExists.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveDirectoryTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var contextMoq = new Mock<ServerCallContext>();
        var service = new FilesService();
        var directoryPath = Path.Combine(FilePaths.TestFolderPath, "test");
        var directoryCreateRequest = new CreateFileOrDirectoryRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        await service.CreateFileOrDirectory(directoryCreateRequest, contextMoq.Object);
        var request = new RemoveFileOrDirectoryRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };

        await service.RemoveFileOrDirectory(request, contextMoq.Object);

        var directoryExists = Directory.Exists(directoryPath);
        directoryExists.Should().BeFalse();
    }
}