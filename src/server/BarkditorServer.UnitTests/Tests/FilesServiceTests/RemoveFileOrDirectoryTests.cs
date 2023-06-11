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
public class RemoveTests
{
    [Fact]
    public async Task RemoveFileTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var contextMoq = new Mock<ServerCallContext>();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "newFile.txt");
        var fileCreateRequest = new CreateRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        await service.Create(fileCreateRequest, contextMoq.Object);
        var request = new RemoveRequest
        {
            Path = filePath,
            IsDirectory = false
        };

        var action = async () => 
            await service.Remove(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
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
        var directoryCreateRequest = new CreateRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        await service.Create(directoryCreateRequest, contextMoq.Object);
        var request = new RemoveRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };

        var action = async () => 
            await service.Remove(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
        var directoryExists = DirectoryWrapper.Exists(directoryPath);
        directoryExists.Should().BeFalse();
    }
    
    [Fact]
    public async Task RemoveFileTest_InvalidArgument()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var contextMoq = new Mock<ServerCallContext>();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "banana.txt");
        var request = new RemoveRequest
        {
            Path = filePath,
            IsDirectory = false
        };

        var action = async () => 
            await service.Remove(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage($"Status(StatusCode=\"InvalidArgument\", Detail=\"Unable to delete a file at path \"{filePath}\"\")");
        var fileExists = File.Exists(filePath);
        fileExists.Should().BeFalse();
    }
    
    [Fact]
    public async Task RemoveDirectoryTest_InvalidArgument()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var contextMoq = new Mock<ServerCallContext>();
        var service = new FilesService();
        var directoryPath = Path.Combine(FilePaths.TestFolderPath, "test");
        var request = new RemoveRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };

        var action = async () => 
            await service.Remove(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage($"Status(StatusCode=\"InvalidArgument\", Detail=\"Unable to delete a folder at path \"{directoryPath}\"\")");
        var directoryExists = DirectoryWrapper.Exists(directoryPath);
        directoryExists.Should().BeFalse();
    }
}