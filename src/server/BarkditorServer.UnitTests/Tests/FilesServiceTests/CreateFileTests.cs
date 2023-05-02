using System.IO;
using System.Runtime.InteropServices;
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
public class CreateFileTests
{
    [Fact]
    public async Task CreateFileTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "testFile.txt");
        var request = new CreateRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Create(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
        var exists = File.Exists(filePath);
        exists.Should().BeTrue();
        var removeFileRequest = new RemoveRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        await service.Remove(removeFileRequest, contextMoq.Object);
    }
    
    [Fact]
    public async Task CreateDirectoryTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var directoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory");
        var request = new CreateRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Create(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
        var exists = DirectoryWrapper.Exists(directoryPath);
        exists.Should().BeTrue();
        var removeDirectoryRequest = new RemoveRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        await service.Remove(removeDirectoryRequest, contextMoq.Object);
    }
    
    [Fact]
    public async Task CreateFileTest_AlreadyExists()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "TestFile1.txt");
        var request = new CreateRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Create(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"AlreadyExists\", Detail=\"This file already exists\")");
        var exists = File.Exists(filePath);
        exists.Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateDirectoryTest_AlreadyExists()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var directoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1");
        var request = new CreateRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Create(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"AlreadyExists\", Detail=\"This directory already exists\")");
        var exists = DirectoryWrapper.Exists(directoryPath);
        exists.Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateDirectoryTest_InvalidArgument()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        string directoryPath;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            directoryPath = Path.Combine(FilePaths.TestFolderPath, "con");
        }
        else
        {
            directoryPath = Path.Combine("/etc", "TestDirectory1");
        }
        
        var request = new CreateRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Create(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage($"Status(StatusCode=\"InvalidArgument\", Detail=\"Unable to create a folder at path \"{directoryPath}\"\")");
        var exists = DirectoryWrapper.Exists(directoryPath);
        exists.Should().BeFalse();
    }
    
    [Fact]
    public async Task CreateFileTest_InvalidArgument()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        string filePath;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            filePath = Path.Combine("C://System32", "avocado.txt");
        }
        else
        {
            filePath = Path.Combine("/etc", "testFile.txt");
        }
        
        var request = new CreateRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.Create(request, contextMoq.Object);
        
        await action.Should().ThrowAsync<RpcException>()
            .WithMessage($"Status(StatusCode=\"InvalidArgument\", Detail=\"Unable to create a file at path \"{filePath}\"\")");
        var exists = DirectoryWrapper.Exists(filePath);
        exists.Should().BeFalse();
    }
}