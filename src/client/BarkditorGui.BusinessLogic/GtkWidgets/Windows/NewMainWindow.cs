using Gtk;

namespace BarkditorGui.BusinessLogic.GtkWidgets.Windows;

public class NewMainWindow : Window
{
    private NewMainWindow(Builder builder, string name) : base(builder.GetPointer(name), false)
    {
        builder.Connect(this);
    }

    public NewMainWindow() : this(new Builder("helloworld.ui"), "MainWindow")
    {
    }
}