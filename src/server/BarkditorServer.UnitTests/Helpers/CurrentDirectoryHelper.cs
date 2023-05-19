using System.IO;
using BarkditorServer.Domain.Constants;

namespace BarkditorServer.UnitTests.Helpers;

internal static class CurrentDirectoryHelper
{
    /// <summary>
    /// Changes current directory to src/server
    /// </summary>
    internal static void SetCurrentDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        if (!currentDirectory.EndsWith("barkditor/src/server/"))
        {
            Directory.SetCurrentDirectory(FilePaths.AppPath);
        }
    }
}