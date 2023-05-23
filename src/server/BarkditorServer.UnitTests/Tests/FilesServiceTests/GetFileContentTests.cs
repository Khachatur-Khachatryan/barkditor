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
public class GetFileContentTests
{
    [Fact]
    public async Task GetPlainTextFile_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var contextMock = new Mock<ServerCallContext>();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "TestFile1.txt");
        var request = new GetFileContentRequest
        {
            Path = filePath
        };
        var expectedContent = await File.ReadAllTextAsync(filePath);
        
        var response = await service.GetFileContent(request, contextMock.Object);

        response.Content.Should().Be(expectedContent);
        response.ContentType.Should().Be(FileContentTypes.PlainText);
    }
    
    [Fact]
    public async Task GetCsharpFile_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var contextMock = new Mock<ServerCallContext>();
        var filePath = 
            Path.Combine(FilePaths.UnitTestsPath, "Tests", "FilesServiceTests", "GetFileContentTests.cs");
        var request = new GetFileContentRequest
        {
            Path = filePath
        };
        var expectedContent = await File.ReadAllTextAsync(filePath);
        
        var response = await service.GetFileContent(request, contextMock.Object);

        response.Content.Should().Be(expectedContent);
        response.ContentType.Should().Be(FileContentTypes.Csharp);
    }
    
    [Fact]
    public async Task GetJsonFile_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var contextMock = new Mock<ServerCallContext>();
        var filePath = 
            Path.Combine(FilePaths.TestFolderPath, "TestFile2.json");
        var request = new GetFileContentRequest
        {
            Path = filePath
        };
        var expectedContent = await File.ReadAllTextAsync(filePath);
        
        var response = await service.GetFileContent(request, contextMock.Object);

        response.Content.Should().Be(expectedContent);
        response.ContentType.Should().Be(FileContentTypes.Json);
    }
    
    [Fact]
    public async Task GetHtmlFile_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new FilesService();
        var contextMock = new Mock<ServerCallContext>();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1", "TestFile4.html");
        var request = new GetFileContentRequest
        {
            Path = filePath
        };
        var expectedContent = await File.ReadAllTextAsync(filePath);
        
        var response = await service.GetFileContent(request, contextMock.Object);

        response.Content.Should().Be(expectedContent);
        response.ContentType.Should().Be(FileContentTypes.Html);
    }
}