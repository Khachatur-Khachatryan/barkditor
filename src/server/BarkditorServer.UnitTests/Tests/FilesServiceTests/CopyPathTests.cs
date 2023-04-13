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
using TextCopy;
using Xunit;

namespace BarkditorServer.UnitTests.Tests.FilesServiceTests;

public class CopyPathTests
{
    [Fact]
    public async Task CopyPath_Success()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            CurrentDirectoryHelper.SetCurrentDirectory();
            var service = new FilesService();
            var filePath = Path.Combine(FilePaths.TestFolderPath, "TestFile1.txt");
            var request = new CopyPathRequest
            {
                Path = filePath
            };
            var contextMoq = new Mock<ServerCallContext>();

            await service.CopyPath(request, contextMoq.Object);
            
            var clipboard = new Clipboard();
            var clipboardData = await clipboard.GetTextAsync();
            clipboardData.Should().Be(filePath);
        }
    }
}