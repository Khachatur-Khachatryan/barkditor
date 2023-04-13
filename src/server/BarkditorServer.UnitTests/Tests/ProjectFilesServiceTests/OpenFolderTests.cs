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
        var request = new OpenFolderRequest
        {
            Path = FilePaths.TestFolderPath
        };
        var expectedFileTree = FileTreeHelper.GetTestFileTree();
        var contextMoq = new Mock<ServerCallContext>();

        var action = async () =>
            await service.OpenFolder(request, contextMoq.Object);
        var response = await action();

        await action.Should().NotThrowAsync();
        response.Files.Should().Be(expectedFileTree);
    }
}