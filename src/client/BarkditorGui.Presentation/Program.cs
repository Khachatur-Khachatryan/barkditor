using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using BarkditorGui.BusinessLogic.GtkWidgets.Windows;
using Gdk;
using GLib;
using Gtk;
using Application = Gtk.Application;
using Settings = Gtk.Settings;
using Task = System.Threading.Tasks.Task;

namespace BarkditorGui.Presentation;

public static class Program
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var appDataPath = Path.GetDirectoryName(Environment.GetEnvironmentVariable("appdata"));
            var gtkThemesPath = Path.Combine(appDataPath!, @"Local\Gtk\3.24.24\share\themes\");
            const string gtkThemeZipFilePath = @"..\..\..\..\themes\W9-Dark.zip";
            var gtkThemePath = $@"{gtkThemesPath}W9-Dark";
            if (!Directory.Exists(gtkThemePath))
            {
                await Task.Run(() =>
                    {
                        Directory.CreateDirectory(gtkThemePath);
                        ZipFile.ExtractToDirectory(gtkThemeZipFilePath, gtkThemePath);
                    }
                );
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                 RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            const string gtkThemesPath = @"~/.themes";
            if (!Directory.Exists(gtkThemesPath))
            {
                Directory.CreateDirectory(gtkThemesPath);
            }
            const string gtkThemePath = @"~/.themes/W9-Dark";
            const string gtkThemeZipFilePath = @"../../../../themes/W9-Dark.zip";

            if (!Directory.Exists(gtkThemePath))
            {
                await Task.Run(() => 
                    {
                        Directory.CreateDirectory(gtkThemePath);
                        ZipFile.ExtractToDirectory(gtkThemeZipFilePath, gtkThemePath);
                    }
                );
            }
        }
        
        Application.Init();
        Settings.Default.ThemeName = "W9-Dark";
        
        var app = new Application("org.BarkditorGui.Presentation", ApplicationFlags.None);
        app.Register(Cancellable.Current);

        var win = new MainWindow();
        app.AddWindow(win);

        async void Action()
        {
            await win.FileSystemViewer.StartWatching();
        }

        Parallel.Invoke(Action);
        
        win.Show();
        Application.Run();
    }
}