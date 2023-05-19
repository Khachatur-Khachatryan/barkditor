using Barkditor.Protobuf;
using BarkditorGui.Utilities.Services;
using Gtk;

namespace BarkditorGui.BusinessLogic.GtkWidgets.Custom;

public class FileContextMenu : Menu
{
    private readonly MenuItem _pasteFileContextMenuItem = new("_Paste");
    private readonly Files.FilesClient _filesClient;
    private readonly TreeView _fileTreeView;
    private readonly TreeStore _fileTreeStore;

    public FileContextMenu(TreeView fileTreeView, TreeStore fileTreeStore, Files.FilesClient filesClient)
    {
        _filesClient = filesClient;
        _fileTreeStore = fileTreeStore;
        _fileTreeView = fileTreeView;
        
        var openInFileManagerMenuItem = new MenuItem("_Open in file manager");
        var removeFileMenuItem = new MenuItem("_Remove");
        var copyFileMenuItem = new MenuItem("_Copy");
        var cutFileMenuItem = new MenuItem("_Cut");
        var copyPathMenuItem = new MenuItem("_Copy path");

        openInFileManagerMenuItem.Activated += FileContextMenuOpenInFileManager_Activated;
        
        removeFileMenuItem.Activated += FileContextMenuRemove_Activated;
        
        copyFileMenuItem.Activated += FileContextMenuCopy_Activated;

        _pasteFileContextMenuItem.Sensitive = false;
        InitializeFileSystemWatcherForTmpCopied();
        _pasteFileContextMenuItem.Activated += FileContextMenuPaste_Activated;
        
        cutFileMenuItem.Activated += FileContextMenuCut_Activated;
        
        copyPathMenuItem.Activated += FileContextMenuCopyPath_Activated;

        AttachToWidget(fileTreeView, null);
        Add(openInFileManagerMenuItem);
        Add(removeFileMenuItem);
        Add(copyFileMenuItem);
        Add(_pasteFileContextMenuItem);
        Add(cutFileMenuItem);
        Add(copyPathMenuItem);
        ShowAll();
    }
    
    private void InitializeFileSystemWatcherForTmpCopied()
    {
        var tmpCopiedPath = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(), "Barkditor", "Copied");
        var fileSystemWatcher = new FileSystemWatcher();
        fileSystemWatcher.BeginInit();

        fileSystemWatcher.NotifyFilter = NotifyFilters.DirectoryName
                                         | NotifyFilters.FileName;

        fileSystemWatcher.Created += (_, _) =>
        {
            _pasteFileContextMenuItem.Sensitive = true;
        };
        fileSystemWatcher.Deleted += (_, _) =>
        {
            if (!Directory.GetFiles(tmpCopiedPath).Any() &&
                !Directory.GetDirectories(tmpCopiedPath).Any())
            {
                _pasteFileContextMenuItem.Sensitive = false;
            }
        };

        fileSystemWatcher.Path = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(), "Barkditor", "Copied");
        fileSystemWatcher.InternalBufferSize = 16384;
        fileSystemWatcher.Filter = "*.*";
        fileSystemWatcher.IncludeSubdirectories = false;
        fileSystemWatcher.EnableRaisingEvents = true;
        
        fileSystemWatcher.EndInit();
    }
    
#region EventHandlers
    
    private void FileContextMenuOpenInFileManager_Activated(object? sender, EventArgs a)
    {
        _fileTreeView.Selection.GetSelected(out var iter);
        var path = (string) _fileTreeStore.GetValue(iter, 2);
        var isDirectory = Directory.Exists(path);
        var request = new OpenInFileManagerRequest
        {
            Path = path,
            IsDirectory = isDirectory
        };

        GrpcRequestSenderService.SendRequest(
            () =>_filesClient.OpenInFileManager(request));
    }
    
    private void FileContextMenuRemove_Activated(object? sender, EventArgs a)
    {
        _fileTreeView.Selection.GetSelected(out var iter);
        var path = (string) _fileTreeStore.GetValue(iter, 2);
        var isDirectory = Directory.Exists(path);
        var request = new RemoveRequest
        {
            Path = path,
            IsDirectory = isDirectory
        };

        GrpcRequestSenderService.SendRequest(
            () => _filesClient.Remove(request));
    }
    
    private void FileContextMenuCopyPath_Activated(object? sender, EventArgs a)
    {
        _fileTreeView.Selection.GetSelected(out var iter);
        var path = (string) _fileTreeStore.GetValue(iter, 2);
        var request = new CopyPathRequest
        {
            Path = path
        };

        GrpcRequestSenderService.SendRequest(
            () => _filesClient.CopyPath(request));
    }

    private void FileContextMenuCopy_Activated(object? sender, EventArgs a)
    {
        _fileTreeView.Selection.GetSelected(out var iter);
        var path = (string) _fileTreeStore.GetValue(iter, 2);
        var isDirectory = (int)_fileTreeStore.GetValue(iter, 3);
        var request = new CopyRequest
        {
            Path = path,
            IsDirectory = isDirectory == 1 
        };

        GrpcRequestSenderService.SendRequest(
            () => _filesClient.Copy(request));
    }

    private void FileContextMenuPaste_Activated(object? sender, EventArgs a)
    {
        _fileTreeView.Selection.GetSelected(out var iter);
        var path = (string) _fileTreeStore.GetValue(iter, 2);
        var isDirectory = (int)_fileTreeStore.GetValue(iter, 3);
        if (isDirectory == 0)
        {
            path = System.IO.Path.GetDirectoryName(path);
        }
        var request = new PasteRequest
        {
            Path = path 
        };

        GrpcRequestSenderService.SendRequest(
            () => _filesClient.Paste(request));
    }

    private void FileContextMenuCut_Activated(object? sender, EventArgs a)
    {
        _fileTreeView.Selection.GetSelected(out var iter);
        var path = (string) _fileTreeStore.GetValue(iter, 2);
        var isDirectory = (int)_fileTreeStore.GetValue(iter, 3);
        var request = new CutRequest()
        {
            Path = path,
            IsDirectory = isDirectory == 1 
        };

        GrpcRequestSenderService.SendRequest(
            () => _filesClient.Cut(request));
    }
    
#endregion
}