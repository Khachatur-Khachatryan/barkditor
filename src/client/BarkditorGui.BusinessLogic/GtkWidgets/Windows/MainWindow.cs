using Barkditor.Protobuf;
using BarkditorGui.BusinessLogic.GtkWidgets.Custom;
using BarkditorGui.Utilities.Services;
using BarkditorGui.BusinessLogic.GtkWidgets.DialogWindows;
using Grpc.Net.Client;
using Gdk;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using Window = Gtk.Window;
using BarkditorGui.Utilities.FileSystem;
using Application = Gtk.Application;
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
    [UI] private readonly Viewport _fileViewport;
    private readonly Files.FilesClient _filesClient;
#pragma warning restore CS0649, CS8618
    private readonly FileViewer _fileViewer;

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
        var projectFilesClient = new ProjectFiles.ProjectFilesClient(channel);
        _filesClient = new Files.FilesClient(channel);
        
        _fileViewer = new FileViewer(_filesClient, projectFilesClient);
        FileSystemViewer = _fileViewer.FileSystemViewer;
        _fileViewport!.Add(_fileViewer);
        _fileViewport.ShowAll();

        DeleteEvent += Window_DeleteEvent;
        _aboutMenuItem!.Activated += AboutButton_Clicked;
        _openFolderItem!.Activated += OpenFolder_Clicked;
        _createFileItem!.Activated += CreateFile_Clicked;
    }

#region GtkEvents

    private static void AboutButton_Clicked(object? sender, EventArgs a)
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
            _fileViewer.OpenFolder(path);
        }
        directoryChooser.Destroy();
    }

    private void CreateFile_Clicked(object? sender, EventArgs a)
    {
        var dialog = new CreateFileDialog(this, _filesClient);
        dialog.Run();
        dialog.Destroy();
    }

    private static void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
    }

#endregion
}