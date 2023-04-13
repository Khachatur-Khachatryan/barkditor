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

        var action = async () =>
            await service.MoveFileOrDirectory(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
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
    public async Task MoveFileTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldFilePath = Path.Combine(FilePaths.TestFolderPath, "TestFile2.json");
        var newFilePath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1", "123.json");
        var request = new MoveFileOrDirectoryRequest
        {
            OldPath = oldFilePath,
            NewPath = newFilePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.MoveFileOrDirectory(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
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

        var action = async () =>
            await service.MoveFileOrDirectory(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
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
    
    [Fact]
    public async Task MoveDirectoryTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1");
        var newDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory2", "123");
        var request = new MoveFileOrDirectoryRequest
        {
            OldPath = oldDirectoryPath,
            NewPath = newDirectoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.MoveFileOrDirectory(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
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
    
    [Fact]
    public async Task RenameFileTest_Unavailable()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldFilePath = Path.Combine(FilePaths.TestFolderPath, "coconut.txt");
        var newFilePath = Path.Combine(FilePaths.TestFolderPath, "123.txt");
        var request = new MoveFileOrDirectoryRequest
        {
            OldPath = oldFilePath,
            NewPath = newFilePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.MoveFileOrDirectory(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"Unavailable\", Detail=\"Unable to rename this file\")");
        var oldFileExists = File.Exists(oldFilePath);
        oldFileExists.Should().BeFalse();
        var newFileExists = File.Exists(newFilePath);
        newFileExists.Should().BeFalse();
        var rollbackRequest = new MoveFileOrDirectoryRequest
        {
            OldPath = newFilePath,
            NewPath = oldFilePath,
            IsDirectory = false
        };
    }
    
    [Fact]
    public async Task MoveFileTest_Unavailable()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldFilePath = Path.Combine(FilePaths.TestFolderPath, "pineapple.json");
        var newFilePath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1", "123.json");
        var request = new MoveFileOrDirectoryRequest
        {
            OldPath = oldFilePath,
            NewPath = newFilePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.MoveFileOrDirectory(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"Unavailable\", Detail=\"Unable to move this file\")");
        var oldFileExists = File.Exists(oldFilePath);
        oldFileExists.Should().BeFalse();
        var newFileExists = File.Exists(newFilePath);
        newFileExists.Should().BeFalse();
        var rollbackRequest = new MoveFileOrDirectoryRequest
        {
            OldPath = newFilePath,
            NewPath = oldFilePath,
            IsDirectory = false
        };
    }

    [Fact]
    public async Task RenameDirectoryTest_Unavailable()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "Cactus");
        var newDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "123");
        var request = new MoveFileOrDirectoryRequest
        {
            OldPath = oldDirectoryPath,
            NewPath = newDirectoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.MoveFileOrDirectory(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"Unavailable\", Detail=\"Unable to rename this directory\")");
        var oldDirectoryExists = Directory.Exists(oldDirectoryPath);
        oldDirectoryExists.Should().BeFalse();
        var newDirectoryExists = Directory.Exists(newDirectoryPath);
        newDirectoryExists.Should().BeFalse();
    }
    
    [Fact]
    public async Task MoveDirectoryTest_Unavailable()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "Calathea");
        var newDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory2", "123");
        var request = new MoveFileOrDirectoryRequest
        {
            OldPath = oldDirectoryPath,
            NewPath = newDirectoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.MoveFileOrDirectory(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"Unavailable\", Detail=\"Unable to move this directory\")");
        var oldDirectoryExists = Directory.Exists(oldDirectoryPath);
        oldDirectoryExists.Should().BeFalse();
        var newDirectoryExists = Directory.Exists(newDirectoryPath);
        newDirectoryExists.Should().BeFalse();
    }
}