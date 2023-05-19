using System.IO;
using Barkditor.Protobuf;
using BarkditorServer.BusinessLogic;
using BarkditorServer.Domain.Constants;

namespace BarkditorServer.UnitTests.Helpers;

internal static class FileTreeHelper
{
    /// <summary>
    /// Provides file tree by path src/server/BarkditorServer.UnitTests/TestFolder
    /// using for unit tests
    /// </summary>
    /// <returns>file tree for tests</returns>
    internal static FileTree GetTestFileTree()
    {
        var testFolderPath = FilePaths.TestFolderPath;
        var fileTree = new FileTree
        {
            Path = testFolderPath,
            IsDirectory = true,
            Name = "TestFolder",
            Files =
            {
                new FileTree
                {
                    Path = Path.Combine(testFolderPath, "TestDirectory1"),
                    IsDirectory = true,
                    Name = "TestDirectory1",
                    Files =
                    {
                        new FileTree
                        {
                            Path = Path.Combine(testFolderPath, "TestDirectory1", "TestFile4.html"),
                            IsDirectory = false,
                            Name = "TestFile4.html"
                        }
                    }
                },
                new FileTree
                {
                    Path = Path.Combine(testFolderPath, "TestDirectory2"),
                    IsDirectory = true,
                    Name = "TestDirectory2",
                    Files =
                    {
                        new FileTree
                        {
                            Path = Path.Combine(testFolderPath, "TestDirectory2", "TestFile3.txt"),
                            IsDirectory = false,
                            Name = "TestFile3.txt"
                        }
                    }
                },
                new FileTree
                {
                    Path = Path.Combine(testFolderPath, "TestFile1.txt"),
                    IsDirectory = false,
                    Name = "TestFile1.txt"
                },
                new FileTree
                {
                    Path = Path.Combine(testFolderPath, "TestFile2.json"),
                    IsDirectory = false,
                    Name = "TestFile2.json"
                },
            }
        };

        return fileTree;
    }
}