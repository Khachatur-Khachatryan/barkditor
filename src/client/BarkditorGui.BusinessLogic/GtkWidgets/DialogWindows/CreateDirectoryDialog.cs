using Barkditor.Protobuf;
using BarkditorGui.Utilities.Services;
using Gdk;
using Gtk;
using Key = Gdk.Key;
using UI = Gtk.Builder.ObjectAttribute;

namespace BarkditorGui.BusinessLogic.GtkWidgets.DialogWindows;

public class CreateDirectoryDialog : Dialog
{
    // These fields are initialized by Glade
#pragma warning disable CS0649
    [UI] private readonly Entry _nameEntry;
    [UI] private readonly Box _errorBox;
    [UI] private readonly Label _errorLabel;
#pragma warning restore CS0649
    private readonly TreeView _fileTreeView;
    private readonly TreeStore _fileTreeStore;
    private readonly Files.FilesClient _filesClient;
    private readonly string _directoryPath;
    
    public CreateDirectoryDialog(Widget parent, TreeView fileTreeView, 
        TreeStore fileTreeStore, Files.FilesClient filesClient) 
        : this(new Builder("CreateDirectoryDialog.glade"), 
            fileTreeView, fileTreeStore, filesClient)
    {
        Parent = parent;
    }
    
#pragma warning disable CS8618
    private CreateDirectoryDialog(Builder builder, TreeView fileTreeView, 
        TreeStore fileTreeStore, Files.FilesClient filesClient) 
        : base(builder.GetRawOwnedObject("CreateDirectoryDialog"))
    {
        GtkWidgetInitService.Initialize(this, builder);
        _fileTreeView = fileTreeView;
        _fileTreeStore = fileTreeStore;
        _filesClient = filesClient;
        _fileTreeView.Selection.GetSelected(out var iter);
        _directoryPath = (string) _fileTreeStore.GetValue(iter, 2);
        
        _nameEntry!.KeyReleaseEvent += NameEntry_KeyRelease;
        DeleteEvent += CloseDialog;
        FocusOutEvent += (_, _) => Hide();
        _nameEntry.Changed += ValidateName;
    }
#pragma warning restore CS8618

    
    private static void CloseDialog(object? sender, EventArgs a)
    {
        Application.Quit();
    }

    private void NameEntry_KeyRelease(object? sender, KeyReleaseEventArgs a)
    {
        if (a.Event.Key is not (Key.KP_Enter or Key.Return))
        {
            return;
        }
        
        if (string.IsNullOrEmpty(_nameEntry.Text))
        {
            Hide();
            return;
        }
        
        var request = new CreateRequest
        {
            Path = System.IO.Path.Combine(_directoryPath, _nameEntry.Text),
            IsDirectory = true
        };
        _filesClient.Create(request);
        Hide();
    }
    
    private void ValidateName(object? sender, EventArgs a)
    {
        var directoryName = _nameEntry.Text;
        var directoryExistsRequest = new ExistsRequest
        {
            Path = System.IO.Path.Combine(_directoryPath, directoryName),
            IsDirectory = true
        };
        var directoryExists = _filesClient.Exists(directoryExistsRequest).Exists;
        
        if (string.IsNullOrEmpty(directoryName) &&
            string.IsNullOrWhiteSpace(directoryName))
        {
            _errorBox.Hide();
            return;
        }
        
        if (directoryExists || 
            string.IsNullOrEmpty(directoryName) || 
            string.IsNullOrWhiteSpace(directoryName))
        {
            _errorLabel.Text = directoryExists ? "Directory already exists" : "Invalid data";
            _errorBox.Show();
            KeyReleaseEvent -= NameEntry_KeyRelease;
            return;
        }
        
        _errorBox.Hide();
        HeightRequest = 100;
    }
}