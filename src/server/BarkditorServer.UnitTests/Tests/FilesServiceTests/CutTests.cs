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
public class CutTests
{
    [Fact]
    public async Task CutFile_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        CreateTempFolderHelper.Create();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "TestFile1.txt");
        var request = new CutRequest
        {
            IsDirectory = false,
            Path = filePath
        };
        var contextMock = new Mock<ServerCallContext>();
        var expectedFileContent = await File.ReadAllTextAsync(filePath);
        
        var action = async () =>
            await service.Cut(request, contextMock.Object);
    
        await action.Should().NotThrowAsync();
        var cutFilePath = Path.Combine(FilePaths.TempCopiedFilesFolderPath, "TestFile1.txt");
        var cutFileExists = File.Exists(cutFilePath);
        var cutFileContent = await File.ReadAllTextAsync(cutFilePath);
        cutFileExists.Should().BeTrue();
        cutFileContent.Should().Be(expectedFileContent);
        var rollbackRequest = new PasteRequest
        {
            Path = FilePaths.TestFolderPath
        };
        await service.Paste(rollbackRequest, contextMock.Object);
        DirectoryWrapper.Clear(FilePaths.TempCopiedFilesFolderPath);
    }
    
    [Fact]
    public async Task CutDirectory_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        CreateTempFolderHelper.Create();
        var service = new FilesService();
        var directoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1");
        var filePath = Path.Combine(directoryPath, "TestFile4.html");
        var request = new CutRequest
        {
            IsDirectory = true,
            Path = directoryPath
        };
        var contextMock = new Mock<ServerCallContext>();
        var expectedFileContent = await File.ReadAllTextAsync(filePath);

        var action = async () =>
            await service.Cut(request, contextMock.Object);

        await action.Should().NotThrowAsync();
        var cutFilePath = 
            Path.Combine(FilePaths.TempCopiedFilesFolderPath, "TestDirectory1", "TestFile4.html");
        var cutFileExists = File.Exists(cutFilePath);
        var cutFileContent = await File.ReadAllTextAsync(cutFilePath);
        cutFileExists.Should().BeTrue();
        cutFileContent.Should().Be(expectedFileContent);
        var rollbackRequest = new PasteRequest
        {
            Path = FilePaths.TestFolderPath
        };
        await service.Paste(rollbackRequest, contextMock.Object);
        DirectoryWrapper.Clear(FilePaths.TempCopiedFilesFolderPath);
    }
}