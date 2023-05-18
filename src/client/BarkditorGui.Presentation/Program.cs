using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
    private const string GtkThemePath = "../../../../themes/inspired/gtk-3.0/gtk.css";
    private const string GtkApplicationId = "org.barkditor.gtk";
    
    [STAThread]
    public static void Main(string[] args)
    {
        Application.Init();
        
        var cssProvider = new CssProvider();
        cssProvider.LoadFromPath(GtkThemePath);
        StyleContext.AddProviderForScreen(Screen.Default, cssProvider, 800);

        var app = new Application(GtkApplicationId, ApplicationFlags.None);
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