using System.IO;
using System.Runtime.InteropServices;
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

        var action = async () =>
            await service.CreateFileOrDirectory(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
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

        var action = async () =>
            await service.CreateFileOrDirectory(request, contextMoq.Object);

        await action.Should().NotThrowAsync();
        var exists = Directory.Exists(directoryPath);
        exists.Should().BeTrue();
        var removeDirectoryRequest = new RemoveFileOrDirectoryRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        await service.RemoveFileOrDirectory(removeDirectoryRequest, contextMoq.Object);
    }
    
    [Fact]
    public async Task CreateFileTest_AlreadyExists()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "TestFile1.txt");
        var request = new CreateFileOrDirectoryRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.CreateFileOrDirectory(request, contextMoq.Object);

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
        var request = new CreateFileOrDirectoryRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.CreateFileOrDirectory(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"AlreadyExists\", Detail=\"This directory already exists\")");
        var exists = Directory.Exists(directoryPath);
        exists.Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateDirectoryTest_Unavailable()
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
        
        var request = new CreateFileOrDirectoryRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.CreateFileOrDirectory(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage($"Status(StatusCode=\"Unavailable\", Detail=\"Unable to create a folder at path \"{directoryPath}\"\")");
        var exists = Directory.Exists(directoryPath);
        exists.Should().BeFalse();
    }
    
    [Fact]
    public async Task CreateFileTest_Unavailable()
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
        
        var request = new CreateFileOrDirectoryRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.CreateFileOrDirectory(request, contextMoq.Object);

        await action.Should().ThrowAsync<RpcException>()
            .WithMessage($"Status(StatusCode=\"Unavailable\", Detail=\"Unable to create a file at path \"{filePath}\"\")");
        var exists = Directory.Exists(filePath);
        exists.Should().BeFalse();
    }
}