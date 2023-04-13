using System.Diagnostics;
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
                Directory.CreateDirectory(path);
                return await Task.FromResult(new Empty());
            }
            
            await File.WriteAllBytesAsync(path, Array.Empty<byte>());

            return await Task.FromResult(new Empty());
        }
        catch (Exception)
        {
            return await Task.FromResult(new Empty());
        }
        
    }

    public override async Task<Empty> MoveFileOrDirectory(MoveFileOrDirectoryRequest request, ServerCallContext ctx)
    {
        var oldPath = request.OldPath;
        var newPath = request.NewPath;
        var isDirectory = request.IsDirectory;

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
        catch(Exception)
        {
            return await Task.FromResult(new Empty());
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

            File.Delete(path);
            return await Task.FromResult(new Empty());
        }
        catch (Exception)
        {
            return await Task.FromResult(new Empty());
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