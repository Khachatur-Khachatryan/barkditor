using Barkditor.Protobuf;
using BarkditorGui.Utilities.FileSystem;
using BarkditorGui.Utilities.Services;
using Gdk;
using Google.Protobuf.WellKnownTypes;
using Gtk;

namespace BarkditorGui.BusinessLogic.GtkWidgets.Custom;

public class FileViewer : Box
{
    private readonly Files.FilesClient _filesClient;
    private readonly ProjectFiles.ProjectFilesClient _projectFilesClient;
    private readonly Menu _fileContextMenu = new();
    private readonly TreeStore _fileTreeStore = new(typeof(string), typeof(Pixbuf), typeof(string), typeof(int));
    private readonly MenuItem _pasteFileContextMenuItem = new("_Paste");
    private readonly TreeView _fileTreeView = new();

    public FileSystemViewer FileSystemViewer { get; }

    public FileViewer(Files.FilesClient filesClient, 
                      ProjectFiles.ProjectFilesClient projectFilesClient) : base(Orientation.Vertical, 0)
    {
        _filesClient = filesClient;
        _projectFilesClient = projectFilesClient;
        FileSystemViewer = 
            new FileSystemViewer(_fileTreeStore, _projectFilesClient);

        FileTreeViewInit();
        FileContextMenuInit();
        LoadSavedProject();
        ShowAll();
    }
    
#region ComponentsInitialization

    private void FileTreeViewInit()
    {
        var fileColumn = new TreeViewColumn();
        
        var sortRenderer = new CellRendererText();
        sortRenderer.Visible = false;
        sortRenderer.Sensitive = false;
        fileColumn.PackStart(sortRenderer, false);
        fileColumn.AddAttribute(sortRenderer, "text", 3);
        
        var pathRenderer = new CellRendererText();
        pathRenderer.Visible = false;
        pathRenderer.Sensitive = false;
        fileColumn.PackStart(pathRenderer, false);
        fileColumn.AddAttribute(pathRenderer, "text", 2);
        
        var iconRenderer = new CellRendererPixbuf();
        fileColumn.PackStart(iconRenderer, false);
        fileColumn.AddAttribute(iconRenderer, "pixbuf", 1);
        iconRenderer.SetPadding(3, 0);
        
        var filenameRenderer = new CellRendererText();
        filenameRenderer.Editable = true;
        filenameRenderer.Edited += FileTreeViewRow_DoubleClicked;
        fileColumn.PackStart(filenameRenderer, true);
        fileColumn.AddAttribute(filenameRenderer, "text", 0);
        
        _fileTreeStore.SetSortColumnId(3, SortType.Descending);

        fileColumn.Title = "Files";
        _fileTreeView.AppendColumn(fileColumn);

        _fileTreeView.Model = _fileTreeStore;
        _fileTreeView.EnableSearch = false;
        _fileTreeView.ButtonReleaseEvent += PopupFileContextMenu;

        PackStart(_fileTreeView, true, true, 0);
    }
    
    private void FileContextMenuInit()
    {
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
        
        cutFileMenuItem.Activated += (_, _) =>
        {
            // TODO: BARKDITOR-GUI-46
        };
        
        copyPathMenuItem.Activated += FileContextMenuCopyPath_Activated;

        _fileContextMenu.AttachToWidget(_fileTreeView, null);
        _fileContextMenu.Add(openInFileManagerMenuItem);
        _fileContextMenu.Add(removeFileMenuItem);
        _fileContextMenu.Add(copyFileMenuItem);
        _fileContextMenu.Add(_pasteFileContextMenuItem);
        _fileContextMenu.Add(cutFileMenuItem);
        _fileContextMenu.Add(copyPathMenuItem);
        _fileContextMenu.ShowAll();
    }

#endregion
    
#region GtkEvents

    private void PopupFileContextMenu(object? sender, ButtonReleaseEventArgs a)
    {
        var s = a.Event;
        if(s.Button == 3 && s.Type == EventType.ButtonRelease)
        {
            _fileContextMenu.PopupAtPointer(null);

            _fileTreeView.ButtonReleaseEvent -= PopupFileContextMenu;
            _fileTreeView.ButtonReleaseEvent += HideFileContextMenu;
        }
    }

    private void HideFileContextMenu(object? sender, ButtonReleaseEventArgs a)
    {
        _fileTreeView.ButtonReleaseEvent += PopupFileContextMenu;
        _fileTreeView.ButtonReleaseEvent -= HideFileContextMenu;
        
        _fileContextMenu.Hide();
    }

    private void FileTreeViewRow_DoubleClicked(object? sender, EditedArgs a)
    {
        // don't confuse with file path
        // https://docs.gtk.org/gtk3/struct.TreePath.html
        var path = new TreePath(a.Path);
        _fileTreeStore.GetIter(out var iter, path);
        var oldPath = (string) _fileTreeStore.GetValue(iter, 2);
        var isDirectory = (int) _fileTreeStore.GetValue(iter, 3);
        var parentDirectoryPath = System.IO.Path.GetDirectoryName(oldPath)!;
        var newPath = System.IO.Path.Combine(parentDirectoryPath, a.NewText);

        var request = new MoveRequest
        {
            OldPath = oldPath,
            NewPath = newPath,
            IsDirectory = isDirectory == 1
        };

        GrpcRequestSenderService.SendRequest(
            () => _filesClient.Move(request));
    }

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

#endregion

#region FileViewer

    private void LoadSavedProject() 
    {
        var empty = new Empty();
        var response = GrpcRequestSenderService.SendRequest(
            () => _projectFilesClient.GetSavedProject(empty));

        if (response is null)
        {
            return;
        }
        
        FileSystemViewer.Watcher.Path = response.Path;
        var projectFiles = response.Files;
        if(projectFiles is null)
        {
            return;
        }    
        ShowProjectFiles(projectFiles);
    }

    public void OpenFolder(string path)
    {
        var request = new OpenFolderRequest
        {
            Path = path
        };
        
        var response = GrpcRequestSenderService.SendRequest(
            () => _projectFilesClient.OpenFolder(request));

        if (response is null)
        {
            return;
        }
        
        var projectFiles = response.Files;
        FileSystemViewer.Watcher.Path = response.Path;
        _fileTreeStore.Clear();

        ShowProjectFiles(projectFiles);
    }

    private void ShowProjectFiles(FileTree fileTree, TreeIter parent) 
    {
        var folderIcon = IconTheme.Default.LoadIcon("folder", (int) IconSize.Menu, 0);
        var fileIcon = IconTheme.Default.LoadIcon("x-office-document", (int) IconSize.Menu, 0);
        
        foreach(var file in fileTree.Files)
        {
            var icon = file.IsDirectory ? folderIcon : fileIcon;
            
            if(file.IsDirectory is false) 
            {
                _fileTreeStore.AppendValues(parent, file.Name, icon, file.Path, 0);
                continue;
            }
            
            var treeIter = _fileTreeStore.AppendValues(parent, file.Name, icon, file.Path, 1);
            ShowProjectFiles(file, treeIter);
        }
    }

    private void ShowProjectFiles(FileTree fileTree) 
    {
        var folderIcon = IconTheme.Default.LoadIcon("folder", (int) IconSize.Menu, 0);
        var fileIcon = IconTheme.Default.LoadIcon("x-office-document", (int) IconSize.Menu, 0);

        foreach(var file in fileTree.Files)
        {
            var icon = file.IsDirectory ? folderIcon : fileIcon;
            
            if(file.IsDirectory is false)
            {
                _fileTreeStore.AppendValues(file.Name, icon, file.Path, 0);
                continue;
            }
            
            var treeIter = _fileTreeStore.AppendValues(file.Name, icon, file.Path, 1);
            ShowProjectFiles(file, treeIter);
        }
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
#endregion

}