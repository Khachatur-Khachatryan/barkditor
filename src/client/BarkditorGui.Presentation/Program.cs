using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BarkditorGui.BusinessLogic.GtkWidgets.Windows;
using GLib;
using Pango;
using Application = Gtk.Application;
using Process = System.Diagnostics.Process;
using Task = System.Threading.Tasks.Task;

namespace BarkditorGui.Presentation;

public static class Program
{
    [STAThread]
    public static async Task Main(string[] args)
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