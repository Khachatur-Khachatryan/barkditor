using System.Threading.Channels;
using Barkditor.Protobuf;
using Gdk;
using Google.Protobuf.WellKnownTypes;
using Gtk;

namespace BarkditorGui.Utilities.FileSystem;

public class FileSystemViewer
{
    private readonly ProjectFiles.ProjectFilesClient _projectFilesClient;
    private readonly TreeStore _fileTreeStore;
    private readonly Channel<FileSystemChange> _channel = Channel.CreateUnbounded<FileSystemChange>();
    private readonly Pixbuf _folderIcon = IconTheme.Default.LoadIcon("folder", (int)IconSize.Menu, 0);
    private readonly Pixbuf _fileIcon = IconTheme.Default.LoadIcon("x-office-document", (int)IconSize.Menu, 0);

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
        // ReSharper disable once FunctionNeverReturns
    }
    
    private void File_Created(FileSystemChange fileSystemChange)
    {
        var projectPath = _projectFilesClient.GetProjectPath(new Empty()).Path;
        var directoryPath = Path.GetDirectoryName(fileSystemChange.FullPath);
        var isDirectory = Directory.Exists(fileSystemChange.FullPath);
        var icon = isDirectory ? _folderIcon : _fileIcon;
        
        if (directoryPath == projectPath)
        {
            Application.Invoke((_, _) =>
            {
                _fileTreeStore.GetIterFirst(out var projectRootIter);
                var createdIter = _fileTreeStore.AppendValues(projectRootIter, 
                    Path.GetFileName(fileSystemChange.Name), icon, 
                    fileSystemChange.FullPath, isDirectory);

                if (isDirectory)
                {
                    ShowFolderContent(createdIter, fileSystemChange.FullPath);
                }
            });
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
        {
            var createdIter = _fileTreeStore.AppendValues(result, 
                Path.GetFileName(fileSystemChange.Name), icon, 
                fileSystemChange.FullPath, isDirectory);

            if (isDirectory)
            {
                ShowFolderContent(createdIter, fileSystemChange.FullPath);
            }
        });
    }

    private void ShowFolderContent(TreeIter folderIter, string folderPath)
    {
        var directoryFiles = Directory.GetFiles(folderPath);

        foreach (var file in directoryFiles)
        {
            _fileTreeStore.AppendValues(folderIter, 
                Path.GetFileName(file), _fileIcon, 
                file, false);
        }
                
        var directoryFolders = Directory.GetDirectories(folderPath);

        foreach (var folder in directoryFolders)
        {
            var subfolderIter = _fileTreeStore.AppendValues(folderIter, 
                Path.GetFileName(folder), _folderIcon, 
                folder, true);
            ShowFolderContent(subfolderIter, folder);
        }
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