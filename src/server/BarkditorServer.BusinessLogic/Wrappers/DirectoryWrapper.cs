namespace BarkditorServer.BusinessLogic.Wrappers;

/// <summary>
/// This class is a static wrapper around Directory class.
/// </summary>
public static class DirectoryWrapper
{
    /// <summary>
    /// Copies a directory and its contents to a new location.
    /// </summary>
    /// <param name="sourceFolderPath">the path of the directory to copy</param>
    /// <param name="destinationFolderPath">the path to the new location for sourceFolderPath</param>
    public static void Copy(string sourceFolderPath, string destinationFolderPath)
    {
        Create(destinationFolderPath);
        var filePaths = Directory.GetFiles(sourceFolderPath);
        foreach (var filePath in filePaths)
        {
            var fileName = Path.GetFileName(filePath);
            var destinationFilePath = Path.Combine(destinationFolderPath, fileName);
            File.Copy(filePath, destinationFilePath);
        }

        var directoryPaths = Directory.GetDirectories(sourceFolderPath);
        foreach (var directoryPath in directoryPaths)
        {
            var directoryName = Path.GetFileName(directoryPath);
            var destinationDirectoryPath = Path.Combine(destinationFolderPath, directoryName!);
            Create(destinationDirectoryPath);
            Copy(directoryPath, destinationDirectoryPath);
        }
    }

    /// <summary>
    /// Creates directory by path.
    /// </summary>
    /// <param name="path">the directory to create</param>
    public static void Create(string path)
    {
        Directory.CreateDirectory(path);
    }

    /// <summary>
    /// Deletes directory by path.
    /// </summary>
    /// <param name="path">the path of the directory to remove</param>
    public static void Delete(string path)
    {
        Directory.Delete(path, true);
    }

    /// <summary>
    /// Clears directory content by path.
    /// </summary>
    /// <param name="path">the path of the directory to clear</param>
    public static void Clear(string path)
    {
        var filePaths = Directory.GetFiles(path);
        foreach (var filePath in filePaths)
        {
            File.Delete(filePath);
        }

        var directoryPaths = Directory.GetDirectories(path);
        foreach (var directoryPath in directoryPaths)
        {
            Delete(directoryPath);
        }
    }

    /// <summary>
    /// Moves a directory and its contents to a new location.
    /// </summary>
    /// <param name="sourceFolderPath">the path of the directory to move</param>
    /// <param name="destinationFolderPath">the path to the new location for sourceFolderPath</param>
    public static void Move(string sourceFolderPath, string destinationFolderPath)
    {
        Directory.Move(sourceFolderPath, destinationFolderPath);
    }

    /// <summary>
    /// Checks whether the given path refers to an existing directory on disk.
    /// </summary>
    /// <param name="path">the path to check</param>
    /// <returns>
    /// true if path refers to an existing directory;
    /// false if the directory does not exist
    /// </returns>
    public static bool Exists(string path)
    {
        return Directory.Exists(path);
    }

    /// <summary>
    /// Checks whether the directory given by path empty
    /// </summary>
    /// <param name="path">the path to check</param>
    /// <returns>
    /// true if the directory is empty or doesn't exists;
    /// false if the directory contains files or other folders
    /// </returns>
    public static bool IsEmpty(string path)
    {
        if (!Exists(path))
        {
            return true;
        }

        if (!Directory.GetFiles(path).Any() &&
            !Directory.GetDirectories(path).Any())
        {
            return true;
        }

        return false;
    }
}