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
public class MoveFileOrDirectoryTests
{
    [Fact]
    public async Task RenameFileTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldFilePath = Path.Combine(FilePaths.TestFolderPath, "TestFile1.txt");
        var newFilePath = Path.Combine(FilePaths.TestFolderPath, "123.txt");
        var request = new MoveFileOrDirectoryRequest
        {
            OldPath = oldFilePath,
            NewPath = newFilePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        await service.MoveFileOrDirectory(request, contextMoq.Object);

        var oldFileExists = File.Exists(oldFilePath);
        oldFileExists.Should().BeFalse();
        var newFileExists = File.Exists(newFilePath);
        newFileExists.Should().BeTrue();
        var rollbackRequest = new MoveFileOrDirectoryRequest
        {
            OldPath = newFilePath,
            NewPath = oldFilePath,
            IsDirectory = false
        };
        await service.MoveFileOrDirectory(rollbackRequest, contextMoq.Object);
    }

    [Fact]
    public async Task RenameDirectoryTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1");
        var newDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "123");
        var request = new MoveFileOrDirectoryRequest
        {
            OldPath = oldDirectoryPath,
            NewPath = newDirectoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        await service.MoveFileOrDirectory(request, contextMoq.Object);

        var oldDirectoryExists = Directory.Exists(oldDirectoryPath);
        oldDirectoryExists.Should().BeFalse();
        var newDirectoryExists = Directory.Exists(newDirectoryPath);
        newDirectoryExists.Should().BeTrue();
        var rollbackRequest = new MoveFileOrDirectoryRequest
        {
            OldPath = newDirectoryPath,
            NewPath = oldDirectoryPath,
            IsDirectory = true
        };
        await service.MoveFileOrDirectory(rollbackRequest, contextMoq.Object);
    }
}