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
public class ExistsTests
{
    [Fact]
    public async Task FileExists_True()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "TestFile1.txt");
        var request = new ExistsRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var response = await service.Exists(request, contextMoq.Object);
        
        response.Exists.Should().BeTrue();
    }
    
    [Fact]
    public async Task FileExists_False()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "file.txt");
        var request = new ExistsRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        var contextMoq = new Mock<ServerCallContext>();

        var response = await service.Exists(request, contextMoq.Object);
        
        response.Exists.Should().BeFalse();
    }
    
    [Fact]
    public async Task DirectoryExists_True()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var directoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1");
        var request = new ExistsRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var response = await service.Exists(request, contextMoq.Object);

        response.Exists.Should().BeTrue();
    }
    
    [Fact]
    public async Task DirectoryExists_False()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var directoryPath = Path.Combine(FilePaths.TestFolderPath, "folder");
        var request = new ExistsRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        var contextMoq = new Mock<ServerCallContext>();

        var response = await service.Exists(request, contextMoq.Object);
        
        response.Exists.Should().BeFalse();
    }
}