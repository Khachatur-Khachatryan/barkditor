using System;
using System.Threading.Tasks;
using BarkditorGui.BusinessLogic.GtkWidgets.Windows;
using GLib;
using Application = Gtk.Application;

namespace BarkditorGui.Presentation;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Application.Init();

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