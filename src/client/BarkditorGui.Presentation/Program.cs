using System;
using BarkditorGui.BusinessLogic.GtkWidgets.Windows;
using Gtk;

namespace BarkditorGui.Presentation;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var application = Application.New("org.Barkditor.core", Gio.ApplicationFlags.FlagsNone);
        application.OnActivate += (_, _) =>
        {
            var window = new MainWindow();
            window.Application = application;
            window.Show();
            window.OnDestroy += (_, _) => application.Quit();
        };
        application.Run();
    }
}