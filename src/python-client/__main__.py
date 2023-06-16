import sys
import gi

from main_window import MainWindow

gi.require_version("Gtk", "4.0")
from gi.repository import GLib, Gtk


class BarkditorApplication(Gtk.Application):
    def __init__(self):
        super().__init__(application_id="org.barkditor.gtk4")
        GLib.set_application_name("Barkditor")

    def do_activate(self):
        window = MainWindow(application=self)
        window.present()


app = BarkditorApplication()
exit_status = app.run(sys.argv)
sys.exit(exit_status)
