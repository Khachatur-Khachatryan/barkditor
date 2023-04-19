using Gtk;

namespace BarkditorGui.BusinessLogic.GtkWidgets.Windows;

public class MainWindow : Window
{
    private MainWindow(Builder builder, string name) : base(builder.GetPointer(name), false)
    {
        builder.Connect(this);
    }

    public MainWindow() : this(new Builder("MainWindow.ui"), "MainWindow")
    {
    }
}