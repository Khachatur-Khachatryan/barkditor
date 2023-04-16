namespace BarkditorGui.BusinessLogic.FileSystem;

public record FileSystemChange
{
    public string FullPath { get; }
    public string Name { get; }
    public string? OldFullPath { get; }
    public WatcherChangeTypes ChangeType { get; }
    
    public FileSystemChange(FileSystemEventArgs e)
    {
        FullPath = e.FullPath;
        Name = e.Name!;
        ChangeType = e.ChangeType;
    }

    public FileSystemChange(RenamedEventArgs e)
    {
        FullPath = e.FullPath;
        Name = e.Name!;
        ChangeType = e.ChangeType;
        OldFullPath = e.OldFullPath;
    }
}