using System.Diagnostics;
using System.Runtime.InteropServices;
using Barkditor.Protobuf;
using BarkditorServer.BusinessLogic.Wrappers;
using BarkditorServer.Domain.Constants;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TextCopy;
#pragma warning disable CS1591

namespace BarkditorServer.BusinessLogic.Services;

public class FilesService : Files.FilesBase
{
    public override async Task<Empty> Create(CreateRequest request, ServerCallContext ctx)
    {
        var path = request.Path;
        var isDirectory = request.IsDirectory;

        try
        {
            if (isDirectory)
            {
                if (DirectoryWrapper.Exists(path))
                {
                    var status = new Status(StatusCode.AlreadyExists,
                        "This directory already exists");
                    throw new RpcException(status);
                }

                DirectoryWrapper.Create(path);
                return await Task.FromResult(new Empty());
            }

            if (File.Exists(path))
            {
                var status = new Status(StatusCode.AlreadyExists,
                    "This file already exists");
                throw new RpcException(status);
            }
            
            await File.WriteAllBytesAsync(path, Array.Empty<byte>());

            return await Task.FromResult(new Empty());
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            var errorMessage = isDirectory
                ? $"Unable to create a folder at path \"{path}\""
                : $"Unable to create a file at path \"{path}\"";
            
            var status = new Status(StatusCode.InvalidArgument, errorMessage);
            throw new RpcException(status);
        }
    }

    public override async Task<Empty> Move(MoveRequest request, ServerCallContext ctx)
    {
        var oldPath = request.OldPath;
        var newPath = request.NewPath;
        var isDirectory = request.IsDirectory;
        var isRenamed = Path.GetDirectoryName(oldPath) == Path.GetDirectoryName(newPath);
        
        try
        {
            if(isDirectory)
            {
                DirectoryWrapper.Move(oldPath, newPath);
            }
            else
            {
                File.Move(oldPath, newPath);
            }
            
            return await Task.FromResult(new Empty());
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            string errorMessage;

            if (isDirectory)
            {
                errorMessage = isRenamed 
                    ? "Unable to rename this directory" 
                    : "Unable to move this directory";
            }
            else
            {
                errorMessage = isRenamed
                    ? "Unable to rename this file"
                    : "Unable to move this file";
            }

            var status = new Status(StatusCode.InvalidArgument, errorMessage);
            throw new RpcException(status);
        }
    }

    public override async Task<Empty> Remove(RemoveRequest request, ServerCallContext ctx)
    {
        var path = request.Path;
        var isDirectory = request.IsDirectory;

        try
        {
            if(isDirectory)
            {
                DirectoryWrapper.Delete(path);
                return await Task.FromResult(new Empty());
            }

            var exists = File.Exists(path);
            if (!exists)
            {
                throw new IOException();
            }
            File.Delete(path);
            return await Task.FromResult(new Empty());
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            var errorMessage = isDirectory
                ? $"Unable to delete a folder at path \"{path}\""
                : $"Unable to delete a file at path \"{path}\"";
            
            var status = new Status(StatusCode.InvalidArgument, errorMessage);
            throw new RpcException(status);
        }
    }

    public override async Task<Empty> OpenInFileManager(OpenInFileManagerRequest request, ServerCallContext ctx)
    {
        var path = request.IsDirectory ? request.Path : Path.GetDirectoryName(request.Path)!;

        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start("explorer.exe", path);
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", path);
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", path);
        }
        
        return await Task.FromResult(new Empty());
    }

    public override async Task<Empty> CopyPath(CopyPathRequest request, ServerCallContext ctx)
    {
        var path = request.Path;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var xdgSessionType = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");
        
            if (xdgSessionType == "x11")
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"echo {path} | xsel --clipboard\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };
                Process.Start(processStartInfo);
            }
            else if (xdgSessionType == "wayland")
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"echo {path} | wl-copy\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(processStartInfo);
            }
            return await Task.FromResult(new Empty());
        }

        var clipboard = new Clipboard();
        
        await clipboard.SetTextAsync(path);
        
        return await Task.FromResult(new Empty());
    }

    public override async Task<ExistsResponse> Exists(ExistsRequest request, ServerCallContext ctx)
    {
        var path = request.Path;
        var isDirectory = request.IsDirectory;

        var response = new ExistsResponse
        {
            Exists = isDirectory ? DirectoryWrapper.Exists(path) : File.Exists(path)
        };

        return await Task.FromResult(response);
    }

    public override async Task<Empty> Copy(CopyRequest request, ServerCallContext ctx)
    {
        DirectoryWrapper.Clear(FilePaths.TempCopiedFilesFolderPath);
        var isDirectory = request.IsDirectory;
        var path = request.Path;

        if (isDirectory)
        {
            var destDirectoryName = Path.GetFileName(path);
            var destDirectoryPath = Path.Combine(FilePaths.TempCopiedFilesFolderPath, destDirectoryName);
            DirectoryWrapper.Copy(path, destDirectoryPath);
        }
        else
        {
            var destFileName = Path.GetFileName(path);
            var destFilePath = Path.Combine(FilePaths.TempCopiedFilesFolderPath, destFileName);
            File.Copy(path, destFilePath);
        }

        return await Task.FromResult(new Empty());
    }

    public override async Task<Empty> Paste(PasteRequest request, ServerCallContext ctx)
    {
        if (DirectoryWrapper.IsEmpty(FilePaths.TempCopiedFilesFolderPath))
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Nothing to paste"));
        }
        var path = request.Path;
        
        DirectoryWrapper.Copy(FilePaths.TempCopiedFilesFolderPath, path);
        
        return await Task.FromResult(new Empty());
    }

    public override async Task<GetFileContentResponse> GetFileContent(GetFileContentRequest request,
        ServerCallContext ctx)
    {
        var path = request.Path;
        string content; 
        
        try
        {
            content = await File.ReadAllTextAsync(path);
        }
        catch (Exception e) when (e is FileNotFoundException or UnauthorizedAccessException)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "File does not exists in system"));
        }

        var fileExtension = Path.GetExtension(path);

        var contentType = fileExtension switch
        {
            ".txt" or ".TXT" => FileContentTypes.PlainText,
            ".cs" => FileContentTypes.Csharp,
            ".json" => FileContentTypes.Json,
            ".html" => FileContentTypes.Html,
            _ => FileContentTypes.PlainText
        };

        var response = new GetFileContentResponse
        {
            Content = content,
            ContentType = contentType
        };

        return response;
    }
}