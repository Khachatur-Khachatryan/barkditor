using Barkditor.Protobuf;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using BarkditorServer.Domain.Constants;

#pragma warning disable CS1591

namespace BarkditorServer.BusinessLogic.Services;

public class ProjectFilesService : ProjectFiles.ProjectFilesBase
{
    public override async Task<FileTreeResponse> OpenFolder(OpenFolderRequest request, ServerCallContext ctx)
    {
        var rootProjectDirectoryInfo = new DirectoryInfo(request.Path);
        var fileTree = GetFileTree(rootProjectDirectoryInfo);
        
        await File.WriteAllTextAsync(FilePaths.ProjectFilesPathTxtPath, 
            rootProjectDirectoryInfo.FullName);

        var response = new FileTreeResponse
        {
            Files = fileTree, 
            Path = request.Path
        };

        return response;
    }

    public override async Task<FileTreeResponse> GetSavedProject(Empty empty, ServerCallContext ctx)
    {
        string path;
        try
        {
            path = await File.ReadAllTextAsync(FilePaths.ProjectFilesPathTxtPath);
        }
        catch
        {
            return new FileTreeResponse();
        }

        var directoryInfo = new DirectoryInfo(path);
        var fileTree = GetFileTree(directoryInfo);
        var response = new FileTreeResponse
        {
            Path = path,
            Files = fileTree
        };

        return response;
    }

    public override async Task<GetProjectPathResponse> GetProjectPath(Empty empty, ServerCallContext ctx)
    {
        var path = await File.ReadAllTextAsync(FilePaths.ProjectFilesPathTxtPath);
        var response = new GetProjectPathResponse
        {
            Path = path
        };

        return response;
    }

    private void GetFileTree(FileTree fileTree, DirectoryInfo directoryInfo) 
    {
        foreach(var projectFolder in directoryInfo.GetDirectories().OrderBy(x => x.Name)) 
        {
            if(DirectoriesToIgnore.IgnoreArray.Contains(projectFolder.Name))
            {
                continue;
            }

            var projectFolderTree = new FileTree
            {
                Name = projectFolder.Name,
                Path = projectFolder.FullName,
                IsDirectory = true
            };
            
            fileTree.Files.Add(projectFolderTree);

            foreach(var projectFile in projectFolder.GetFiles().OrderBy(x => x.Name))
            {
                var projectFileTree = new FileTree
                {
                    Name = projectFile.Name,
                    Path = projectFile.FullName,
                    IsDirectory = false
                };
                projectFolderTree.Files.Add(projectFileTree);
            }
            
            GetFileTree(projectFolderTree, projectFolder);
        }
    }

    private FileTree GetFileTree(DirectoryInfo directoryInfo)
    {
        var fileTree = new FileTree
        {
            Path = directoryInfo.FullName,
            Name = directoryInfo.Name,
            IsDirectory = true
        };

        GetFileTree(fileTree, directoryInfo);
        
        foreach(var projectFile in directoryInfo.GetFiles().OrderBy(x => x.Name))
        {
            var projectFileTree = new FileTree
            {
                Name = projectFile.Name,
                Path = projectFile.FullName,
                IsDirectory = false
            };
            fileTree.Files.Add(projectFileTree);
        }

        return fileTree;
    }
}
