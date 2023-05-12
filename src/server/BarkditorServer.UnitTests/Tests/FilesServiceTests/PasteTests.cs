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
public class PasteTests
{
    [Fact]
    public async Task FilePaste_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        CreateTempFolderHelper.Create();
        var service = new FilesService();
        var contextMock = new Mock<ServerCallContext>();
        var filePath = Path.Combine(FilePaths.TestFolderPath, "TestFile1.txt");
        var pastedFilePath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1");
        var copyRequest = new CopyRequest
        {
            Path = filePath,
            IsDirectory = false
        };
        await service.Copy(copyRequest, contextMock.Object);
        var pasteRequest = new PasteRequest
        {
            Path = pastedFilePath
        };
    
        var action = async () =>
            await service.Paste(pasteRequest, contextMock.Object);
    
        await action.Should().NotThrowAsync();
        File.Delete(Path.Combine(pastedFilePath, "TestFile1.txt"));
        DirectoryWrapper.Clear(FilePaths.TempCopiedFilesFolderPath);
    }
    
    [Fact]
    public async Task DirectoryPaste_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        CreateTempFolderHelper.Create();
        var service = new FilesService();
        var contextMock = new Mock<ServerCallContext>();
        var directoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1");
        var pastedDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory2");
        var copyRequest = new CopyRequest
        {
            Path = directoryPath,
            IsDirectory = true
        };
        await service.Copy(copyRequest, contextMock.Object);
        var pasteRequest = new PasteRequest
        {
            Path = pastedDirectoryPath
        };
    
        var action = async () =>
            await service.Paste(pasteRequest, contextMock.Object);
    
        await action.Should().NotThrowAsync();
        DirectoryWrapper.Delete(Path.Combine(pastedDirectoryPath, "TestDirectory1"));
        DirectoryWrapper.Clear(FilePaths.TempCopiedFilesFolderPath);
    }

    [Fact]
    public async Task Paste_NothingToPaste()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        CreateTempFolderHelper.Create();
        var service = new FilesService();
        var contextMock = new Mock<ServerCallContext>();
        var pastedDirectoryPath = Path.Combine(FilePaths.TestFolderPath, "TestDirectory1");
        var pasteRequest = new PasteRequest
        {
            Path = pastedDirectoryPath
        };
    
        var action = async () =>
            await service.Paste(pasteRequest, contextMock.Object);
    
        await action.Should().ThrowAsync<RpcException>()
            .WithMessage("Status(StatusCode=\"FailedPrecondition\", Detail=\"Nothing to paste\")");
    }
}