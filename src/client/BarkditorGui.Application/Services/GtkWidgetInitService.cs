using Gdk;
using Gtk;

public static class GtkWidgetInitService 
{
    public static void Initialize(Widget widget, Builder builder)
    {
        var cssProvider = new CssProvider();
        var stylePath = System.IO.Path.GetFullPath(
            System.IO.Path.Combine(AppContext.BaseDirectory, "../../../../BarkditorGui.BusinessLogic/Css/style.css"));
        cssProvider.LoadFromPath(stylePath);
        builder.Autoconnect(widget);
        StyleContext.AddProviderForScreen(Screen.Default, cssProvider, 800);
    }
}