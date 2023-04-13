using Barkditor.Protobuf;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using Window = Gtk.Window;

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
    [UI] private readonly Box _pathErrorBox;
    [UI] private readonly Button _createButton;
    [UI] private readonly Button _cancelButton;
#pragma warning restore CS0649
    private readonly Files.FilesClient _filesClient;

#endregion

    public CreateFileDialog(Window parent, Files.FilesClient filesClient) 
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
#pragma warning restore CS8602
        DeleteEvent += CloseDialog;
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
        catch (Exception)
        {
            Hide();        
        }
    }
    
    private void CancelButton_Clicked(object? sender, EventArgs a)
    {
        Hide();
    }

    private void CloseDialog(object? sender, EventArgs a)
    {
        Application.Quit();
    }

#endregion

    private void ValidateNameAndPath(object? sender, EventArgs a)
    {
        var path = _pathEntry.Text;
        var name = _nameEntry.Text;
        
        if (!Directory.Exists(path) || 
            string.IsNullOrEmpty(name) || 
            string.IsNullOrWhiteSpace(name))
        {
            _createButton.Sensitive = false;
            _directoryChooserButton.UnselectAll();
            _pathErrorBox.Show();
            return;
        }
        
        _pathErrorBox.Hide();
        _directoryChooserButton.SetCurrentFolder(path);
        _createButton.Sensitive = true;
    }
}