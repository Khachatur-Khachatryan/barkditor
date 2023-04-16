using Barkditor.Protobuf;
using BarkditorGui.Utilities.Services;
using Grpc.Core;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace BarkditorGui.BusinessLogic.GtkWidgets.DialogWindows;

public class CreateFileDialog : Dialog
{

#region Fields

    // These fields are initialized by Glade
#pragma warning disable CS0649
    [UI] private readonly FileChooserButton _directoryChooserButton;
    [UI] private readonly Entry _nameEntry;
    [UI] private readonly Entry _pathEntry;
    [UI] private readonly EntryBuffer _pathEntryBuffer;
    [UI] private readonly Box _errorBox;
    [UI] private readonly Label _errorLabel;
    [UI] private readonly Button _createButton;
    [UI] private readonly Button _cancelButton;
#pragma warning restore CS0649
    private readonly Files.FilesClient _filesClient;

#endregion

    public CreateFileDialog(Widget parent, Files.FilesClient filesClient) 
        : this(new Builder("CreateFileDialog.glade"))
    {
        _filesClient = filesClient;
        Parent = parent;
    }
    
#pragma warning disable CS8618
    private CreateFileDialog(Builder builder) : base(builder.GetRawOwnedObject("CreateFileDialog"))
    {
        GtkWidgetInitService.Initialize(this, builder);

#pragma warning disable CS8602
        _directoryChooserButton.SelectionChanged += DirectoryChooser_SelectionChanged;
        _pathEntry.Changed += ValidateNameAndPath;
        _nameEntry.Changed += ValidateNameAndPath;
        _cancelButton.Clicked += CancelButton_Clicked;
        _createButton.Clicked += CreateButton_Clicked;
        DeleteEvent += CloseDialog;
#pragma warning restore CS8602
    }
#pragma warning restore CS8618

#region GtkEvents

    private void DirectoryChooser_SelectionChanged(object? sender, EventArgs a)
    {
        _pathEntryBuffer.Text = _directoryChooserButton.Filename;
    }

    private void CreateButton_Clicked(object? sender, EventArgs a)
    {
        var filename = _nameEntry.Text;
        var directoryPath = _pathEntry.Text;
        var filePath = System.IO.Path.Combine(directoryPath, filename);
        var request = new CreateFileOrDirectoryRequest
        {
            Path = filePath,
            IsDirectory = false
        };

        try
        {
            _filesClient.CreateFileOrDirectory(request);        
        }
        catch (RpcException)
        {
            Hide();        
        }
    }
    
    private void CancelButton_Clicked(object? sender, EventArgs a)
    {
        Hide();
    }

    private static void CloseDialog(object? sender, EventArgs a)
    {
        Application.Quit();
    }

#endregion

    private void ValidateNameAndPath(object? sender, EventArgs a)
    {
        var path = _pathEntry.Text;
        var name = _nameEntry.Text;
        var fileExistsRequest = new ExistsRequest
        {
            Path = System.IO.Path.Combine(path, name),
            IsDirectory = false
        };
        var fileExists = _filesClient.Exists(fileExistsRequest).Exists;
        var directoryExistsRequest = new ExistsRequest
        {
            Path = path,
            IsDirectory = true
        };
        var directoryExists = _filesClient.Exists(directoryExistsRequest).Exists;
        
        if (string.IsNullOrEmpty(name) &&
            string.IsNullOrWhiteSpace(name) &&
            string.IsNullOrEmpty(path) &&
            string.IsNullOrWhiteSpace(path))
        {
            _createButton.Sensitive = false;
            _directoryChooserButton.UnselectAll();
            _errorBox.Hide();
            return;
        }
        
        if (fileExists ||
            !directoryExists || 
            string.IsNullOrEmpty(name) || 
            string.IsNullOrWhiteSpace(name))
        {
            _createButton.Sensitive = false;
            _directoryChooserButton.UnselectAll();
            _errorLabel.Text = fileExists ? "File already exists" : "Invalid data";
            _errorBox.Show();
            return;
        }
        
        _errorBox.Hide();
        _directoryChooserButton.SetCurrentFolder(path);
        _createButton.Sensitive = true;
    }
}