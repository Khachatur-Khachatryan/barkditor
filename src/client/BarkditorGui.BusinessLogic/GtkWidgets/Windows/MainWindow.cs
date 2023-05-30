using Barkditor.Protobuf;
using BarkditorGui.BusinessLogic.GtkWidgets.Custom;
using BarkditorGui.Utilities.Services;
using Grpc.Net.Client;
using Gdk;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using Window = Gtk.Window;
using BarkditorGui.Utilities.FileSystem;
using GtkSource;
using Application = Gtk.Application;
using File = System.IO.File;
using MenuItem = Gtk.MenuItem;

namespace BarkditorGui.BusinessLogic.GtkWidgets.Windows;
public class MainWindow : Window
{

#region FieldsAndProperties

    // These fields are initialized by Glade or Protobuf
#pragma warning disable CS0649, CS8618
    [UI] private readonly MenuItem _aboutMenuItem;
    [UI] private readonly MenuItem _openFolderItem;
    [UI] private readonly MenuItem _openFileItem;
    [UI] private readonly Viewport _fileViewport;
    [UI] private readonly Notebook _codeNotebook;
#pragma warning restore CS0649, CS8618
    private readonly SourceView _codeSourceView;
    private readonly FileViewer _fileViewer;
    private readonly Files.FilesClient _filesClient;
    
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

        // TODO: add language server support
        _codeSourceView = new SourceView();
        _codeSourceView.AutoIndent = true;
        _codeSourceView.TabWidth = 4;
        _codeSourceView.InsertSpacesInsteadOfTabs = true;
        _codeSourceView.Buffer = new GtkSource.Buffer();
        _codeSourceView.Buffer.HighlightSyntax = true;
        _codeSourceView.Buffer.HighlightMatchingBrackets = true;
        _codeSourceView.Buffer.MaxUndoLevels = -1;
        _codeSourceView.Editable = true;
        _codeSourceView.ShowLineNumbers = true;
        _codeSourceView.LeftMargin = 5;
        _codeSourceView.SmartBackspace = true;
        _codeSourceView.InsertSpacesInsteadOfTabs = true;
        _codeSourceView.ShowLineMarks = true;
        _codeSourceView.ShowRightMargin = true;
        _codeSourceView.IndentWidth = 0;
        _codeSourceView.StyleContext.AddClass("editor");
        var styleSchemeManager = new StyleSchemeManager();
        styleSchemeManager.AppendSearchPath("../../../../dist/");
        _codeSourceView.Buffer.StyleScheme = styleSchemeManager.GetScheme("darcula");
        _codeSourceView.ShowAll();
        var scrolledWindow = new ScrolledWindow();
        scrolledWindow.Add(_codeSourceView);
        scrolledWindow.ShowAll();
        _codeNotebook!.Add(scrolledWindow);
        
        _fileViewer = new FileViewer(_filesClient, projectFilesClient, _codeSourceView);
        FileSystemViewer = _fileViewer.FileSystemViewer;
        _fileViewport!.Add(_fileViewer);
        _fileViewport.ShowAll();

        DeleteEvent += Window_DeleteEvent;
        _aboutMenuItem!.Activated += AboutButton_Clicked;
        _openFileItem!.Activated += OpenFile_Clicked;
        _openFolderItem!.Activated += OpenFolder_Clicked;
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

    private void OpenFile_Clicked(object? sender, EventArgs a)
    {
        var dialog = new FileChooserNative("Open File", 
            this, FileChooserAction.Open, 
            "Open", "Cancel");
        dialog.SetCurrentFolder(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        var responseCode = dialog.Run();
        if (responseCode == (int) ResponseType.Accept)
        {
            var request = new GetFileContentRequest
            {
                Path = dialog.Filename
            };

            var response = _filesClient.GetFileContent(request);

            _codeSourceView.Buffer.Text = response.Content;
        }
        dialog.Destroy();
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

    private static void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
    }

#endregion
}