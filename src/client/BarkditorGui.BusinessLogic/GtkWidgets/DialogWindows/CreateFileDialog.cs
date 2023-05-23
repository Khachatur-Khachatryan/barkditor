using Barkditor.Protobuf;
using BarkditorGui.Utilities.Services;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace BarkditorGui.BusinessLogic.GtkWidgets.DialogWindows;

public class CreateFileDialog : Dialog
{
    // These fields are initialized by Glade
#pragma warning disable CS0649
    [UI] private readonly Entry _nameEntry;
    [UI] private readonly Box _errorBox;
    [UI] private readonly Label _errorLabel;
#pragma warning restore CS0649
    private readonly Files.FilesClient _filesClient;
    private readonly string _directoryPath;

    public CreateFileDialog(Widget parent, Files.FilesClient filesClient, ITreeModel fileTreeStore, 
        TreeView fileTreeView) 
        : this(new Builder("CreateFileDialog.glade"), filesClient, fileTreeStore, 
            fileTreeView)
    {
        Parent = parent;
    }
    
#pragma warning disable CS8618
    private CreateFileDialog(Builder builder, Files.FilesClient filesClient, ITreeModel fileTreeStore, 
        TreeView fileTreeView) 
        : base(builder.GetRawOwnedObject("CreateFileDialog"))
    {
        GtkWidgetInitService.Initialize(this, builder);
        
        _filesClient = filesClient;
        fileTreeView.Selection.GetSelected(out var iter);
        var isDirectory = (bool)fileTreeStore.GetValue(iter, 3);
        if (isDirectory)
        {
            _directoryPath = (string)fileTreeStore.GetValue(iter, 2);
        }
        else
        {
            fileTreeStore.IterParent(out var parent, iter);
            _directoryPath = (string)fileTreeStore.GetValue(parent, 2);
        }
        
        _nameEntry!.KeyReleaseEvent += NameEntry_KeyRelease;
        _nameEntry.Changed += ValidateName;
        FocusOutEvent += (_, _) => Hide();
        DeleteEvent += CloseDialog;
    }
#pragma warning restore CS8618

#region GtkEventHandlers

    private void NameEntry_KeyRelease(object? sender, KeyReleaseEventArgs a)
    {
        if (a.Event.Key is not (Gdk.Key.KP_Enter or Gdk.Key.Return))
        {
            return;
        }
        
        var fileName = _nameEntry.Text;
        
        if (string.IsNullOrEmpty(fileName))
        {
            Hide();
            return;
        }
        
        var fileExistsRequest = new ExistsRequest
        {
            Path = System.IO.Path.Combine(_directoryPath, fileName),
            IsDirectory = false
        };
        var fileExists = GrpcRequestSenderService.SendRequest(
                () => _filesClient.Exists(fileExistsRequest))!
                .Exists;
        
        if (fileExists || string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }
        
        var request = new CreateRequest
        {
            Path = System.IO.Path.Combine(_directoryPath, _nameEntry.Text),
            IsDirectory = false
        };

        GrpcRequestSenderService.SendRequest(() => 
                _filesClient.Create(request));
        Hide();
    }
    
    private void ValidateName(object? sender, EventArgs a)
    {
        var fileName = _nameEntry.Text;
        var fileExistsRequest = new ExistsRequest
        {
            Path = System.IO.Path.Combine(_directoryPath, fileName),
            IsDirectory = false
        };
        var fileExists = GrpcRequestSenderService.SendRequest(
                () => _filesClient.Exists(fileExistsRequest))!
                .Exists;
        
        if (string.IsNullOrEmpty(fileName) &&
            string.IsNullOrWhiteSpace(fileName))
        {
            _errorBox.Hide();
            return;
        }
        
        if (fileExists || 
            string.IsNullOrEmpty(fileName) || 
            string.IsNullOrWhiteSpace(fileName))
        {
            _errorLabel.Text = fileExists ? "File already exists" : "Invalid data";
            _errorBox.Show();
            return;
        }
        
        _errorBox.Hide();
        HeightRequest = 100;
    }

    private static void CloseDialog(object? sender, EventArgs a)
    {
        Application.Quit();
    }

#endregion

}