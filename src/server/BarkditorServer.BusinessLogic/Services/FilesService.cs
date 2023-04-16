using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using Barkditor.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TextCopy;
#pragma warning disable CS1591

namespace BarkditorServer.BusinessLogic.Services;

public class FilesService : Files.FilesBase
{
    public override async Task<Empty> CreateFileOrDirectory(CreateFileOrDirectoryRequest request, ServerCallContext ctx)
    {
        var path = request.Path;
        var isDirectory = request.IsDirectory;

        try
        {
            if (isDirectory)
            {
                if (Directory.Exists(path))
                {
                    var status = new Status(StatusCode.AlreadyExists,
                        "This directory already exists");
                    throw new RpcException(status);
                }

                Directory.CreateDirectory(path);
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

    public override async Task<Empty> MoveFileOrDirectory(MoveFileOrDirectoryRequest request, ServerCallContext ctx)
    {
        var oldPath = request.OldPath;
        var newPath = request.NewPath;
        var isDirectory = request.IsDirectory;
        var isRenamed = Path.GetDirectoryName(oldPath) == Path.GetDirectoryName(newPath);
        
        try
        {
            if(isDirectory)
            {
                Directory.Move(oldPath, newPath);
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

    public override async Task<Empty> RemoveFileOrDirectory(RemoveFileOrDirectoryRequest request, ServerCallContext ctx)
    {
        var path = request.Path;
        var isDirectory = request.IsDirectory;

        try
        {
            if(isDirectory)
            {
                Directory.Delete(path);
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
        var clipboard = new Clipboard();
        
        await clipboard.SetTextAsync(path);
        
        return await Task.FromResult(new Empty());
    }
}