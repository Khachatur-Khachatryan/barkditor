using System.IO;
using System.Threading.Tasks;
using Barkditor.Protobuf;
using BarkditorServer.BusinessLogic.Services;
using BarkditorServer.BusinessLogic.Wrappers;
using BarkditorServer.Domain.Constants;
using BarkditorServer.UnitTests.Helpers;
using FluentAssertions;
using Grpc.Core;
using Moq;
using Xunit;

namespace BarkditorServer.UnitTests.Tests.FilesServiceTests;

[Collection("Sequential")]
public class MoveTests
{
    [Fact]
    public async Task RenameFileTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldFilePath = Path.Combine(FilePaths.TestFolderPath, "TestFile1.txt");
        var newFilePath = Path.Combine(FilePaths.TestFolderPath, "123.txt");
        var request = new MoveRequest
        {
            OldPath = oldFilePath,
            NewPath = newFilePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Move(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
        var oldFileExists = File.Exists(oldFilePath);
        oldFileExists.Should().BeFalse();
        var newFileExists = File.Exists(newFilePath);
        newFileExists.Should().BeTrue();
        var rollbackRequest = new MoveRequest
        {
            OldPath = newFilePath,
            NewPath = oldFilePath,
            IsDirectory = false
        };
        await service.Move(rollbackRequest, contextMoq.Object);
    }
    
    [Fact]
    public async Task MoveFileTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldFilePath = Path.Combine(FilePaths.TestFolderPath, "TestFile2.json");
        var newFilePath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1", "123.json");
        var request = new MoveRequest
        {
            OldPath = oldFilePath,
            NewPath = newFilePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Move(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
        var oldFileExists = File.Exists(oldFilePath);
        oldFileExists.Should().BeFalse();
        var newFileExists = File.Exists(newFilePath);
        newFileExists.Should().BeTrue();
        var rollbackRequest = new MoveRequest
        {
            OldPath = newFilePath,
            NewPath = oldFilePath,
            IsDirectory = false
        };
        await service.Move(rollbackRequest, contextMoq.Object);
    }

    [Fact]
    public async Task RenameDirectoryTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1");
        var newDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "123");
        var request = new MoveRequest
        {
            OldPath = oldDirectoryPath,
            NewPath = newDirectoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Move(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
        var oldDirectoryExists = DirectoryWrapper.Exists(oldDirectoryPath);
        oldDirectoryExists.Should().BeFalse();
        var newDirectoryExists = DirectoryWrapper.Exists(newDirectoryPath);
        newDirectoryExists.Should().BeTrue();
        var rollbackRequest = new MoveRequest
        {
            OldPath = newDirectoryPath,
            NewPath = oldDirectoryPath,
            IsDirectory = true
        };
        await service.Move(rollbackRequest, contextMoq.Object);
    }
    
    [Fact]
    public async Task MoveDirectoryTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1");
        var newDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory2", "123");
        var request = new MoveRequest
        {
            OldPath = oldDirectoryPath,
            NewPath = newDirectoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Move(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
        var oldDirectoryExists = DirectoryWrapper.Exists(oldDirectoryPath);
        oldDirectoryExists.Should().BeFalse();
        var newDirectoryExists = DirectoryWrapper.Exists(newDirectoryPath);
        newDirectoryExists.Should().BeTrue();
        var rollbackRequest = new MoveRequest
        {
            OldPath = newDirectoryPath,
            NewPath = oldDirectoryPath,
            IsDirectory = true
        };
        await service.Move(rollbackRequest, contextMoq.Object);
    }
    
    [Fact]
    public async Task RenameFileTest_InvalidArgument()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldFilePath = Path.Combine(FilePaths.TestFolderPath, "coconut.txt");
        var newFilePath = Path.Combine(FilePaths.TestFolderPath, "123.txt");
        var request = new MoveRequest
        {
            OldPath = oldFilePath,
            NewPath = newFilePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Move(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"InvalidArgument\", Detail=\"Unable to rename this file\")");
        var oldFileExists = File.Exists(oldFilePath);
        oldFileExists.Should().BeFalse();
        var newFileExists = File.Exists(newFilePath);
        newFileExists.Should().BeFalse();
    }
    
    [Fact]
    public async Task MoveFileTest_InvalidArgument()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldFilePath = Path.Combine(FilePaths.TestFolderPath, "pineapple.json");
        var newFilePath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1", "123.json");
        var request = new MoveRequest
        {
            OldPath = oldFilePath,
            NewPath = newFilePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Move(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"InvalidArgument\", Detail=\"Unable to move this file\")");
        var oldFileExists = File.Exists(oldFilePath);
        oldFileExists.Should().BeFalse();
        var newFileExists = File.Exists(newFilePath);
        newFileExists.Should().BeFalse();
    }

    [Fact]
    public async Task RenameDirectoryTest_InvalidArgument()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "Cactus");
        var newDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "123");
        var request = new MoveRequest
        {
            OldPath = oldDirectoryPath,
            NewPath = newDirectoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Move(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"InvalidArgument\", Detail=\"Unable to rename this directory\")");
        var oldDirectoryExists = DirectoryWrapper.Exists(oldDirectoryPath);
        oldDirectoryExists.Should().BeFalse();
        var newDirectoryExists = DirectoryWrapper.Exists(newDirectoryPath);
        newDirectoryExists.Should().BeFalse();
    }
    
    [Fact]
    public async Task MoveDirectoryTest_InvalidArgument()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var oldDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "Calathea");
        var newDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory2", "123");
        var request = new MoveRequest
        {
            OldPath = oldDirectoryPath,
            NewPath = newDirectoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Move(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"InvalidArgument\", Detail=\"Unable to move this directory\")");
        var oldDirectoryExists = DirectoryWrapper.Exists(oldDirectoryPath);
        oldDirectoryExists.Should().BeFalse();
        var newDirectoryExists = DirectoryWrapper.Exists(newDirectoryPath);
        newDirectoryExists.Should().BeFalse();
    }
}