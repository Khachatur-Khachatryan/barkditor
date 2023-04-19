using System.Threading.Channels;
using Barkditor.Protobuf;
using Google.Protobuf.WellKnownTypes;
namespace BarkditorGui.Utilities.FileSystem;

public class FileSystemViewer
{
    private readonly ProjectFiles.ProjectFilesClient _projectFilesClient;
    private readonly Channel<FileSystemChange> _channel = Channel.CreateUnbounded<FileSystemChange>();
    public FileSystemWatcher Watcher { get; } = new();
    
    public FileSystemViewer(ProjectFiles.ProjectFilesClient projectFilesClient)
    {
        _projectFilesClient = projectFilesClient;
        
        Watcher.BeginInit();

        Watcher.NotifyFilter = NotifyFilters.DirectoryName
                               | NotifyFilters.FileName;

        Watcher.Created += async (_, e) =>
        {
            var fsChange = new FileSystemChange(e);
            await _channel.Writer.WriteAsync(fsChange);
        };
        Watcher.Deleted += async (_, e) =>
        {
            var fsChange = new FileSystemChange(e);
            await _channel.Writer.WriteAsync(fsChange);
        };
        Watcher.Renamed += async (_, e) =>
        {
            var renameOrMovedFsChange = new FileSystemChange(e);
            await _channel.Writer.WriteAsync(renameOrMovedFsChange);
        };

        Watcher.InternalBufferSize = 16384;
        Watcher.Filter = "*.*";
        Watcher.IncludeSubdirectories = true;
        Watcher.EnableRaisingEvents = true;
        
        Watcher.EndInit();
    }

    public async Task StartWatching()
    {
        while (true)
        {
            var item = await _channel.Reader.ReadAsync();

            switch (item.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    File_Created(item);
                    break;
                case WatcherChangeTypes.Deleted:
                    File_Deleted(item);
                    break;
                case WatcherChangeTypes.Renamed:
                    File_Renamed(item);
                    break;
                default:
                    continue;
            }
        }
    }
    
    private void File_Created(FileSystemChange fileSystemChange)
    {
    }

    private void File_Deleted(FileSystemChange fileSystemChange)
    {
    }

    private void File_Renamed(FileSystemChange fileSystemChange)
    {
    }
 
}