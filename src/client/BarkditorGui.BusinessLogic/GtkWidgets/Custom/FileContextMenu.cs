using Barkditor.Protobuf;
using BarkditorGui.BusinessLogic.GtkWidgets.DialogWindows;
using BarkditorGui.Utilities.Services;
using Google.Protobuf.WellKnownTypes;
using Gtk;

namespace BarkditorGui.BusinessLogic.GtkWidgets.Custom;

public class FileContextMenu : Menu
{
    private readonly MenuItem _pasteFileContextMenuItem = new("_Paste");
    private readonly MenuItem _removeFileContextMenuItem = new("_Remove");
    private readonly Files.FilesClient _filesClient;
    private readonly ProjectFiles.ProjectFilesClient _projectFilesClient;
    private readonly TreeView _fileTreeView;
    private readonly TreeStore _fileTreeStore;

    public FileContextMenu(TreeView fileTreeView, TreeStore fileTreeStore, Files.FilesClient filesClient,
        ProjectFiles.ProjectFilesClient projectFilesClient)
    {
        _filesClient = filesClient;
        _projectFilesClient = projectFilesClient;
        _fileTreeStore = fileTreeStore;
        _fileTreeView = fileTreeView;

        InitializeFileSystemWatcherForTmpCopied();

        var createFileMenuItem = new MenuItem("_Create file");
        var createDirectoryMenuItem = new MenuItem("_Create directory");
        var openInFileManagerMenuItem = new MenuItem("_Open in file manager");
        var renameFileMenuItem = new MenuItem("_Rename");
        var copyFileMenuItem = new MenuItem("_Copy");
        var copyPathMenuItem = new MenuItem("_Copy path");
        _pasteFileContextMenuItem.Sensitive = false;

        createFileMenuItem.Activated += FileContextMenuCreateFile_Activated;
        createDirectoryMenuItem.Activated += FileContextMenuCreateDirectory_Activated;
        openInFileManagerMenuItem.Activated += FileContextMenuOpenInFileManager_Activated;
        renameFileMenuItem.Activated += FileContextMenuRename_Activated;
        _removeFileContextMenuItem.Activated += FileContextMenuRemove_Activated;
        copyFileMenuItem.Activated += FileContextMenuCopy_Activated;
        _pasteFileContextMenuItem.Activated += FileContextMenuPaste_Activated;
        copyPathMenuItem.Activated += FileContextMenuCopyPath_Activated;

        AttachToWidget(fileTreeView, null);
        Add(createFileMenuItem);
        Add(createDirectoryMenuItem);
        Add(openInFileManagerMenuItem);
        Add(renameFileMenuItem);
        Add(_removeFileContextMenuItem);
        Add(copyFileMenuItem);
        Add(_pasteFileContextMenuItem);
        Add(copyPathMenuItem);

        PoppedUp += FileContextMenu_PoppedUp;
        
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

    private void FileContextMenu_PoppedUp(object? sender, PoppedUpArgs a)
    {
        _fileTreeView.Selection.GetSelected(out var iter);
        var projectPath = _projectFilesClient.GetProjectPath(new Empty()).Path;
        var path = (string)_fileTreeStore.GetValue(iter, 2);

        if (path == projectPath)
        {
            _removeFileContextMenuItem.Sensitive = false;
            return;
        }

        _removeFileContextMenuItem.Sensitive = true;
    }

    private void FileContextMenuOpenInFileManager_Activated(object? sender, EventArgs a)
    {
        _fileTreeView.Selection.GetSelected(out var iter);
        var path = (string) _fileTreeStore.GetValue(iter, 2);
        var isDirectory = (bool) _fileTreeStore.GetValue(iter, 3);
        var request = new OpenInFileManagerRequest
        {
            Path = path,
            IsDirectory = isDirectory
        };

        GrpcRequestSenderService.SendRequest(
            () =>_filesClient.OpenInFileManager(request));
    }

    private void FileContextMenuCreateFile_Activated(object? sender, EventArgs a)
    {
        var dialog = new CreateFileDialog(this, _filesClient, _fileTreeStore, _fileTreeView);
        dialog.Run();
        dialog.Destroy();
    }
    
    private void FileContextMenuCreateDirectory_Activated(object? sender, EventArgs a)
    {
        var dialog = new CreateDirectoryDialog(this, _fileTreeView, _fileTreeStore, _filesClient);
        dialog.Run();
        dialog.Destroy();
    }
    
    private void FileContextMenuRename_Activated(object? sender, EventArgs a)
    {
        var dialog = new RenameFileDialog(this, _filesClient, _fileTreeStore, _fileTreeView);
        dialog.Run();
        dialog.Destroy();
    }
    
    private void FileContextMenuRemove_Activated(object? sender, EventArgs a)
    {
        _fileTreeView.Selection.GetSelected(out var iter);
        var path = (string) _fileTreeStore.GetValue(iter, 2);
        var isDirectory = (bool) _fileTreeStore.GetValue(iter, 3);
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
        var isDirectory = (bool) _fileTreeStore.GetValue(iter, 3);
        var request = new CopyRequest
        {
            Path = path,
            IsDirectory = isDirectory
        };

        GrpcRequestSenderService.SendRequest(
            () => _filesClient.Copy(request));
    }

    private void FileContextMenuPaste_Activated(object? sender, EventArgs a)
    {
        _fileTreeView.Selection.GetSelected(out var iter);
        var path = (string) _fileTreeStore.GetValue(iter, 2);
        var isDirectory = (bool) _fileTreeStore.GetValue(iter, 3);
        if (!isDirectory)
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

    
#endregion
}