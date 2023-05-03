using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using BarkditorGui.BusinessLogic.GtkWidgets.Windows;
using Gdk;
using GLib;
using Gtk;
using Application = Gtk.Application;

namespace BarkditorGui.Presentation;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Application.Init();
        var cssProvider = new CssProvider();
        cssProvider.LoadFromPath("../../../../themes/Marwaita-Pop_os-master/Marwaita Pop_os/gtk-3.0/gtk-dark.css");
        StyleContext.AddProviderForScreen(Screen.Default, cssProvider, 800);
        
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