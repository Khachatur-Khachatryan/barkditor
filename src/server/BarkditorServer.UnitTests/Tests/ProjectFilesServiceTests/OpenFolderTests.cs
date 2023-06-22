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

namespace BarkditorServer.UnitTests.Tests.ProjectFilesServiceTests;

[Collection("Sequential")]
public class OpenFolderTests
{
    [Fact]
    public async Task OpenFolderTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new ProjectFilesService();
        var expectedPath = FilePaths.TestFolderPath; 
        var request = new SetProjectPathRequest
        {
            Path = expectedPath
        };
        var contextMoq = new Mock<ServerCallContext>();
        
        await service.SetProjectPath(request, contextMoq.Object);

        var projectPath = await File.ReadAllTextAsync(FilePaths.ProjectPathTxtPath);
        projectPath.Should().Be(expectedPath);
    }
}