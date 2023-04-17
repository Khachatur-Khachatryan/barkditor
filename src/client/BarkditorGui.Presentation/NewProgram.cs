using System;
using BarkditorGui.BusinessLogic.GtkWidgets.Windows;
using Gtk;

namespace BarkditorGui.Presentation;

public static class NewProgram
{
    [STAThread]
    public static void Main(string[] args)
    {
        var application = Application.New("org.HelloWorld.core", Gio.ApplicationFlags.FlagsNone);
        application.OnActivate += (sender, _) =>
        {
            var window = new NewMainWindow();
            window.Application = application;
            window.Show();
            window.OnDestroy += (_, _) => application.Quit();
        };
        application.Run();
    }
}