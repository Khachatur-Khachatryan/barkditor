using System;
using BarkditorServer.BusinessLogic.Wrappers;
using BarkditorServer.Domain.Constants;

namespace BarkditorServer.UnitTests.Helpers;

internal static class CreateTempFolderHelper
{
    /// <summary>
    /// Creates project temporary folders
    /// </summary>
    internal static void Create()
    {
        try
        {
            DirectoryWrapper.Create(FilePaths.TempFolderPath);
            DirectoryWrapper.Create(FilePaths.TempCopiedFilesFolderPath);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}