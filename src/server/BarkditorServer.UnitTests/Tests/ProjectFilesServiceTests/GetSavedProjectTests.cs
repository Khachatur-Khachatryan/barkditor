using System.Threading.Tasks;
using Barkditor.Protobuf;
using BarkditorServer.BusinessLogic.Services;
using BarkditorServer.Domain.Constants;
using BarkditorServer.UnitTests.Helpers;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Moq;
using Xunit;

namespace BarkditorServer.UnitTests.Tests.ProjectFilesServiceTests;

[Collection("Sequential")]
public class GetSavedProjectTests
{
    [Fact]
    public async Task GetSavedProjectTest_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new ProjectFilesService();
        var request = new OpenFolderRequest
        {
            Path = FilePaths.TestFolderPath
        };
        var contextMoq = new Mock<ServerCallContext>();
        await service.OpenFolder(request, contextMoq.Object);
        var expectedFileTree = FileTreeHelper.GetTestFileTree();
        var empty = new Empty();

        var response = await service.GetSavedProject(empty, contextMoq.Object);

        response.Files.Should().Be(expectedFileTree);
    }
}