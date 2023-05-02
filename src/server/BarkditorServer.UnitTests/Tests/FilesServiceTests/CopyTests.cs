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
public class CopyTests
{
    [Fact]
    public async Task CopyFile_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        CreateTempFolderHelper.Create();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "TestFile1.txt");
        var request = new CopyRequest
        {
            IsDirectory = false,
            Path = filePath
        };
        var contextMock = new Mock<ServerCallContext>();
        var expectedFileContent = await File.ReadAllTextAsync(filePath);
    
        var action = async () =>
            await service.Copy(request, contextMock.Object);
    
        await action.Should().NotThrowAsync();
        var copiedFilePath = Path.Combine(FilePaths.TempCopiedFilesFolderPath, "TestFile1.txt");
        var copiedFileExists = File.Exists(copiedFilePath);
        var copiedFileContent = await File.ReadAllTextAsync(copiedFilePath);
        copiedFileExists.Should().BeTrue();
        copiedFileContent.Should().Be(expectedFileContent);
        DirectoryWrapper.Clear(FilePaths.TempCopiedFilesFolderPath);
    }

    [Fact]
    public async Task CopyDirectory_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        CreateTempFolderHelper.Create();
        var service = new FilesService();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "TestFile1.txt");
        var request = new CopyRequest
        {
            IsDirectory = true,
            Path = FilePaths.TestFolderPath
        };
        var contextMock = new Mock<ServerCallContext>();
        var expectedFileContent = await File.ReadAllTextAsync(filePath);

        var action = async () =>
            await service.Copy(request, contextMock.Object);

        await action.Should().NotThrowAsync();
        var copiedFilePath = 
            Path.Combine(FilePaths.TempCopiedFilesFolderPath, "TestFolder", "TestFile1.txt");
        var copiedFileExists = File.Exists(copiedFilePath);
        var copiedFileContent = await File.ReadAllTextAsync(copiedFilePath);
        copiedFileExists.Should().BeTrue();
        copiedFileContent.Should().Be(expectedFileContent);
        DirectoryWrapper.Clear(FilePaths.TempCopiedFilesFolderPath);
    }
}