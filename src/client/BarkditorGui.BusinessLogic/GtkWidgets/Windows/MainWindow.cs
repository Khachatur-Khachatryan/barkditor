using Barkditor.Protobuf;
using BarkditorGui.Utilities.Services;
using BarkditorGui.BusinessLogic.GtkWidgets.DialogWindows;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Gdk;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using Window = Gtk.Window;
using BarkditorGui.Utilities.FileSystem;
using GLib;
using Application = Gtk.Application;
using Menu = Gtk.Menu;
using MenuItem = Gtk.MenuItem;

namespace BarkditorGui.BusinessLogic.GtkWidgets.Windows;
public class MainWindow : Window
{

#region FieldsAndProperties

    // These fields are initialized by Glade or Protobuf
#pragma warning disable CS0649, CS8618
    [UI] private readonly MenuItem _aboutMenuItem;
    [UI] private readonly MenuItem _openFolderItem;
    [UI] private readonly MenuItem _createFileItem;
    [UI] private readonly TreeView _fileTreeView;
    private readonly Files.FilesClient _filesClient;
    private readonly ProjectFiles.ProjectFilesClient _projectFilesClient;
#pragma warning restore CS0649, CS8618
    private readonly TreeStore _fileTreeStore = new(typeof(string), typeof(Pixbuf), typeof(string), typeof(int));
    private readonly Menu _fileContextMenu = new();
    
    public FileSystemViewer FileSystemViewer { get; }
    
#endregion

    public MainWindow() : this(new Builder("MainWindow.glade")) { }
    private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
    {
        GtkWidgetInitService.Initialize(this, builder);

        var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
        {
            MaxReceiveMessageSize = null,
            MaxSendMessageSize = null
        });
        _projectFilesClient = new ProjectFiles.ProjectFilesClient(channel);
        _filesClient = new Files.FilesClient(channel);

        FileSystemViewer = 
            new FileSystemViewer(_fileTreeStore, _projectFilesClient);
        
        FileTreeViewInit();
        LoadSavedProject();
        FileContextMenuInit();
        
#pragma warning disable CS8602
        DeleteEvent += Window_DeleteEvent;
        _aboutMenuItem.Activated += AboutButton_Clicked;
        _openFolderItem.Activated += OpenFolder_Clicked;
        _createFileItem.Activated += CreateFile_Clicked;
        _fileTreeView.ButtonReleaseEvent += PopupFileContextMenu;
#pragma warning restore CS8602
    }

#region GtkComponentsInitialization

    private void FileContextMenuInit()
    {
        var openInFileManagerMenuItem = new MenuItem("_Open in file manager");
        var removeFileMenuItem = new MenuItem("_Remove");
        var copyFileMenuItem = new MenuItem("_Copy");
        var pasteFileMenuItem = new MenuItem("_Paste");
        var cutFileMenuItem = new MenuItem("_Cut");
        var copyPathMenuItem = new MenuItem("_Copy path");

        openInFileManagerMenuItem.Activated += FileContextMenuOpenInFileManager_Activated;
        
        removeFileMenuItem.Activated += FileContextMenuRemove_Activated;
        
        copyFileMenuItem.Activated += FileContextMenuCopy_Activated;

        pasteFileMenuItem.Activated += (_, _) =>
        {
            // TODO: BARKDITOR-GUI-47
        };
        
        cutFileMenuItem.Activated += (_, _) =>
        {
            // TODO: BARKDITOR-GUI-46
        };
        
        copyPathMenuItem.Activated += FileContextMenuCopyPath_Activated;

        _fileContextMenu.AttachToWidget(_fileTreeView, null);
        _fileContextMenu.Add(openInFileManagerMenuItem);
        _fileContextMenu.Add(removeFileMenuItem);
        _fileContextMenu.Add(copyFileMenuItem);
        _fileContextMenu.Add(pasteFileMenuItem);
        _fileContextMenu.Add(cutFileMenuItem);
        _fileContextMenu.Add(copyPathMenuItem);
        _fileContextMenu.ShowAll();
    }

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
        iconRenderer.CellBackground = "#333333";
        fileColumn.PackStart(iconRenderer, false);
        fileColumn.AddAttribute(iconRenderer, "pixbuf", 1);
        iconRenderer.SetPadding(3, 0);
        
        var filenameRenderer = new CellRendererText();
        filenameRenderer.Editable = true;
        filenameRenderer.Edited += FileTreeViewRow_DoubleClicked;
        filenameRenderer.CellBackground = "#333333";
        fileColumn.PackStart(filenameRenderer, true);
        fileColumn.AddAttribute(filenameRenderer, "text", 0);
        
        _fileTreeStore.SetSortColumnId(3, SortType.Descending);

        fileColumn.Title = "Files";
        _fileTreeView.AppendColumn(fileColumn);

        _fileTreeView.Model = _fileTreeStore;
        _fileTreeView.EnableSearch = false;
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

    private void AboutButton_Clicked(object? sender, EventArgs a)
    {
        var logoPath = System.IO.Path.GetFullPath(
            System.IO.Path.Combine(AppContext.BaseDirectory, "../../../../../../img/barkditor-logo.svg"));
        var loader = new PixbufLoader();
        loader.Write(File.ReadAllBytes(logoPath));
        loader.Close();

        var aboutDialog = new AboutDialog();
        aboutDialog.Resizable = false;
        aboutDialog.DefaultWidth = 500;
        aboutDialog.DefaultHeight = 325;
        aboutDialog.Logo = loader.Pixbuf;
        aboutDialog.WrapLicense = true;
        aboutDialog.ProgramName = "Barkditor";
        aboutDialog.Copyright = "© 2023 Khachatur Khachatryan";
        aboutDialog.Comments = "Lightweight, cross-platform, and powerful code editor";
        aboutDialog.License = "Barkditor is free software; you may distribute and modify it under the terms of the third version" 
            + "of the GNU General Public License published by Free Software Foundation.\n"
            + "\nBarkditor supplied without any warranty. Details in GNU General Public License (third version)\n"
            + "\nYou should receive a copy of the GNU General Public License along with Barkditor."
            + "Otherwise, write about it to: Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.";
        aboutDialog.WebsiteLabel = "View source code";
        aboutDialog.Website = "https://github.com/Khachatur-Khachatryan/barkditor";
        aboutDialog.Authors = new[]
        {
            "Khachatur Khachatryan, main developer"
        };
        aboutDialog.Run();
        aboutDialog.Destroy();
    }

    private void OpenFolder_Clicked(object? sender, EventArgs a) 
    {
        var directoryChooser = 
            new FileChooserDialog("Choose directory to open",
            this, FileChooserAction.SelectFolder,
            "Cancel", ResponseType.Cancel,
            "Open", ResponseType.Ok);

        var directoryChooserResponse = directoryChooser.Run();
        if(directoryChooserResponse == (int) ResponseType.Ok)
        {
            var path = directoryChooser.Filename;
            OpenFolder(path);
        }
        directoryChooser.Destroy();
    }

    private void CreateFile_Clicked(object? sender, EventArgs a)
    {
        var dialog = new CreateFileDialog(this, _filesClient);
        dialog.Run();
        dialog.Destroy();
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

    private static void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
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

    private void OpenFolder(string path)
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

#endregion

}