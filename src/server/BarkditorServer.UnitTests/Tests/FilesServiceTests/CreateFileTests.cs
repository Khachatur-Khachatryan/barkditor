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
public class CreateFileTests
{
    [Fact]
    public async Task CreateFileTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "testFile.txt");
        var request = new CreateFileOrDirectoryRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        await service.CreateFileOrDirectory(request, contextMoq.Object);

        var exists = File.Exists(filePath);
        exists.Should().BeTrue();
        var removeFileRequest = new RemoveFileOrDirectoryRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        await service.RemoveFileOrDirectory(removeFileRequest, contextMoq.Object);
    }
    
    [Fact]
    public async Task CreateDirectoryTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var directoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory");
        var request = new CreateFileOrDirectoryRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        await service.CreateFileOrDirectory(request, contextMoq.Object);

        var exists = Directory.Exists(directoryPath);
        exists.Should().BeTrue();
        var removeDirectoryRequest = new RemoveFileOrDirectoryRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        await service.RemoveFileOrDirectory(removeDirectoryRequest, contextMoq.Object);
    }
}