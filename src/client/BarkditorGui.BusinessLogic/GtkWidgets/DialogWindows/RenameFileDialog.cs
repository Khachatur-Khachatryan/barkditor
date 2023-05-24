using Barkditor.Protobuf;
using BarkditorGui.Utilities.Services;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace BarkditorGui.BusinessLogic.GtkWidgets.DialogWindows;

public class RenameFileDialog : Dialog
{
    // These fields are initialized by Glade
#pragma warning disable CS0649
    [UI] private readonly Entry _nameEntry;
    [UI] private readonly EntryBuffer _nameEntryBuffer;
    [UI] private readonly Box _errorBox;
    [UI] private readonly Label _errorLabel;
#pragma warning restore CS0649
    private readonly Files.FilesClient _filesClient;
    private readonly string _directoryPath;
    private readonly string _oldFilePath;
    private readonly bool _isDirectory;
    private bool _exists;

    public RenameFileDialog(Widget parent, Files.FilesClient filesClient, ITreeModel fileTreeStore, 
        TreeView fileTreeView) 
        : this(new Builder("RenameFileDialog.glade"), filesClient, fileTreeStore, 
            fileTreeView)
    {
        Parent = parent;
    }

#pragma warning disable CS8618
    private RenameFileDialog(Builder builder, Files.FilesClient filesClient, ITreeModel fileTreeStore, 
        TreeView fileTreeView) 
        : base(builder.GetRawOwnedObject("RenameFileDialog"))
    {
        GtkWidgetInitService.Initialize(this, builder);

        _filesClient = filesClient;

        fileTreeView.Selection.GetSelected(out var iter);
        _nameEntryBuffer!.Text = (string)fileTreeStore.GetValue(iter, 0); 
        _isDirectory = (bool)fileTreeStore.GetValue(iter, 3);

        if (_isDirectory)
        {
            _oldFilePath = (string)fileTreeStore.GetValue(iter, 2);
            _directoryPath = (string)fileTreeStore.GetValue(iter, 2);
        }
        else
        {
            fileTreeStore.IterParent(out var parent, iter);
            _oldFilePath = (string)fileTreeStore.GetValue(iter, 2);
            _directoryPath = (string)fileTreeStore.GetValue(parent, 2);
        }

        ValidateName(this, EventArgs.Empty);

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
        
        if (_exists || string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        string newPath;

        if (_isDirectory)
        {
            newPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(_directoryPath)!,
                _nameEntry.Text);
        }
        else
        {
            newPath = System.IO.Path.Combine(_directoryPath, _nameEntry.Text);
        }

        var request = new MoveRequest
        {
            NewPath = newPath,
            OldPath = _oldFilePath,
            IsDirectory = _isDirectory
        };

        GrpcRequestSenderService.SendRequest(() =>
            _filesClient.Move(request));
        Hide();
    }

    private void ValidateName(object? sender, EventArgs a)
    {
        var fileName = _nameEntry.Text;

        if (string.IsNullOrEmpty(fileName) &&
            string.IsNullOrWhiteSpace(fileName))
        {
            _exists = false;
            _errorBox.Hide();
            return;
        }
        
        if (_isDirectory)
        {
            var directoryExistsRequest = new ExistsRequest
            {
                Path = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(_directoryPath)!, fileName),
                IsDirectory = true
            };
            var directoryExists = GrpcRequestSenderService.SendRequest(
                    () => _filesClient.Exists(directoryExistsRequest))!
                    .Exists;

            if (directoryExists ||
                string.IsNullOrEmpty(fileName) || 
                string.IsNullOrWhiteSpace(fileName))
            {
                _exists = true;
                _errorLabel.Text = directoryExists ? "Directory already exists" : "Invalid data";
                _errorBox.Show();
                return;
            }

            _exists = false;
            _errorBox.Hide();
            HeightRequest = 100;
            return;
        }

        var fileExistRequest = new ExistsRequest
        {
            Path = System.IO.Path.Combine(_directoryPath, fileName),
            IsDirectory = false
        };
        var fileExists = _filesClient.Exists(fileExistRequest).Exists;

        if (fileExists ||
            string.IsNullOrEmpty(fileName) || 
            string.IsNullOrWhiteSpace(fileName))
        {
            _exists = true;
            _errorLabel.Text = fileExists ? "File already exists" : "Invalid data";
            _errorBox.Show();
            return;
        }

        _exists = false;
        _errorBox.Hide();
        HeightRequest = 100;
    }

    private static void CloseDialog(object? sender, EventArgs a)
    {
        Application.Quit();
    }

#endregion

}