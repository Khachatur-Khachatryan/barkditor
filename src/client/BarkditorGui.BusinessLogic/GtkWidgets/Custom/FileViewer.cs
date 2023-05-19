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
    private readonly FileContextMenu _fileContextMenu;
    private readonly TreeStore _fileTreeStore = new(typeof(string), typeof(Pixbuf), typeof(string), typeof(int));
    private readonly TreeView _fileTreeView = new();

    public FileSystemViewer FileSystemViewer { get; }

    public FileViewer(Files.FilesClient filesClient, 
                      ProjectFiles.ProjectFilesClient projectFilesClient) 
        : base(Orientation.Vertical, 0)
    {
        _filesClient = filesClient;
        _projectFilesClient = projectFilesClient;
        
        FileSystemViewer = new FileSystemViewer(_fileTreeStore, _projectFilesClient);
        _fileContextMenu = new FileContextMenu(_fileTreeView, _fileTreeStore, _filesClient);

        // file tree view init start
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
        
        // file tree view init end
        
        LoadSavedProject();
        ShowAll();
    }
    
#region GtkEventHandlers

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

#endregion

#region FileTreeOutput

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
    
    private void ShowProjectFiles(FileTree fileTree) 
    {
        var folderIcon = IconTheme.Default.LoadIcon("folder", (int) IconSize.Menu, 0);
        var fileIcon = IconTheme.Default.LoadIcon("x-office-document", (int) IconSize.Menu, 0);
        var rootProjectDirectoryIcon = 
            IconTheme.Default.LoadIcon("folder-templates", (int)IconSize.Menu, 0);
        
        // add project root directory
        var rootProjectTreeIter = _fileTreeStore.AppendValues(fileTree.Name, rootProjectDirectoryIcon, 1);
        
        
        foreach(var file in fileTree.Files)
        {
            var icon = file.IsDirectory ? folderIcon : fileIcon;
            
            if(file.IsDirectory is false)
            {
                _fileTreeStore.AppendValues(rootProjectTreeIter, file.Name, icon, file.Path, 0);
                continue;
            }
            
            var treeIter = _fileTreeStore.AppendValues(rootProjectTreeIter, file.Name, icon, file.Path, 1);
            ShowProjectFiles(file, treeIter);
        }
        
        var rootProjectTreePath = _fileTreeStore.GetPath(rootProjectTreeIter);
        _fileTreeView.ExpandRow(rootProjectTreePath, false);
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

#endregion

}