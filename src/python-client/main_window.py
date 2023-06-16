import gi

gi.require_version("Gtk", "4.0")
from gi.repository import Gtk


class MainWindow(Gtk.ApplicationWindow):
    def __init__(self, application):
        super().__init__(application=application, title="Barkditor")
        self.set_size_request(900, 700)
