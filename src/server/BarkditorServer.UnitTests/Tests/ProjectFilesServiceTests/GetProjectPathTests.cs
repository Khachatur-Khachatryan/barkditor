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
public class GetProjectPathTests
{
    [Fact]
    public async Task GetProjectPath_Success()
    {
        CurrentDirectoryHelper.SetCurrentDirectory();
        var service = new ProjectFilesService();
        var expectedPath = FilePaths.TestFolderPath;
        var openFolderRequest = new SetProjectPathRequest
        {
            Path = FilePaths.TestFolderPath
        };
        var contextMoq = new Mock<ServerCallContext>();
        await service.SetProjectPath(openFolderRequest, contextMoq.Object);
        var empty = new Empty();

        var response = await service.GetProjectPath(empty, contextMoq.Object);

        response.Path.Should().Be(expectedPath);
    }
}