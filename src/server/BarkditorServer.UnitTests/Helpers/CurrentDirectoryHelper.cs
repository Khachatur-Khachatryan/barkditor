using System.IO;
using BarkditorServer.Domain.Constants;

namespace BarkditorServer.UnitTests.Helpers;

internal static class CurrentDirectoryHelper
{
    internal static void SetCurrentDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        if (!currentDirectory.EndsWith("barkditor/src/server/BarkditorServer.Presentation"))
        {
            Directory.SetCurrentDirectory(FilePaths.AppPath);
        }
    }
}