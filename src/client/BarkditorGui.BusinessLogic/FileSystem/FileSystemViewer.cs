using System.Threading.Channels;
using Barkditor.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Gtk;
namespace BarkditorGui.BusinessLogic.FileSystem;

public class FileSystemViewer
{
    private readonly ProjectFiles.ProjectFilesClient _projectFilesClient;
    private readonly TreeStore _fileTreeStore;
    private readonly Channel<FileSystemChange> _channel = Channel.CreateUnbounded<FileSystemChange>();
    public FileSystemWatcher Watcher { get; } = new();
    
    public FileSystemViewer(TreeStore fileTreeStore,
        ProjectFiles.ProjectFilesClient projectFilesClient)
    {
        _projectFilesClient = projectFilesClient;
        _fileTreeStore = fileTreeStore;
        
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
        var folderIcon = IconTheme.Default.LoadIcon("folder", (int)IconSize.Menu, 0);
        var fileIcon = IconTheme.Default.LoadIcon("x-office-document", (int)IconSize.Menu, 0);
        
        var projectPath = _projectFilesClient.GetProjectPath(new Empty()).Path;
        var directoryPath = Path.GetDirectoryName(fileSystemChange.FullPath);
        var isDirectory = Directory.Exists(fileSystemChange.FullPath);
        var sortType = isDirectory ? 1 : 0;
        var icon = isDirectory ? folderIcon : fileIcon;
        
        if (directoryPath == projectPath)
        {
            Application.Invoke((_, _) => 
                _fileTreeStore.AppendValues(Path.GetFileName(fileSystemChange.Name), icon, fileSystemChange.FullPath, sortType));
            return;
        }

        var iterList = new List<TreeIter>();
        _fileTreeStore.Foreach((_, _, iter) =>
        {
            iterList.Add(iter);
            return false;
        });

        var result = iterList.FirstOrDefault(x => 
        {
            var iterPath = (string)_fileTreeStore.GetValue(x, 2);
            return iterPath == directoryPath;
        });
        
        Application.Invoke((_, _) =>
            _fileTreeStore.AppendValues(result, Path.GetFileName(fileSystemChange.Name), 
                icon, fileSystemChange.FullPath, sortType));
    }

    private void File_Deleted(FileSystemChange fileSystemChange)
    {
        var iterList = new List<TreeIter>();
        _fileTreeStore.Foreach((_, _, iter) =>
        {
            iterList.Add(iter);
            return false;
        });

        var deletedIter = iterList.FirstOrDefault(x =>
        {
            var iterPath = (string)_fileTreeStore.GetValue(x, 2);
            return iterPath == fileSystemChange.FullPath;
        });

        Application.Invoke((_, _) => _fileTreeStore.Remove(ref deletedIter));
    }

    private void File_Renamed(FileSystemChange fileSystemChange)
    {
        var isDirectory = Directory.Exists(fileSystemChange.FullPath);
        var filename = Path.GetFileName(fileSystemChange.Name);
        var iterList = new List<TreeIter>();
        _fileTreeStore.Foreach((_, _, iter) =>
        {
            iterList.Add(iter);
            return false;
        });

        var renamedIter = iterList.FirstOrDefault(x =>
        {
            var iterPath = (string)_fileTreeStore.GetValue(x, 2);
            return iterPath == fileSystemChange.OldFullPath;
        });
        
        Application.Invoke((_, _) =>
        {
            _fileTreeStore.SetValue(renamedIter, 2, fileSystemChange.FullPath);
            _fileTreeStore.SetValue(renamedIter, 0, filename);
        });
        
        if (isDirectory)
        {
            var renamedIterChildren = iterList.Where(x =>
            {
                var iterPath = (string)_fileTreeStore.GetValue(x, 2);
                return iterPath.Contains(fileSystemChange.OldFullPath!);
            });

            Application.Invoke((_, _) =>
            {
                foreach (var iter in renamedIterChildren)
                {
                    var iterPath = (string)_fileTreeStore.GetValue(iter, 2);
                    var s = iterPath.Replace(fileSystemChange.OldFullPath!, string.Empty);
                    var newPath = fileSystemChange.FullPath + s;
                
                    _fileTreeStore.SetValue(iter, 2, newPath);
                }    
            });
        }
    }
 
}