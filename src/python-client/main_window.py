import gi

gi.require_version("Gtk", "4.0")
from gi.repository import Gtk, Gio


# https://docs.gtk.org/gtk3/class.ApplicationWindow.html
class MainWindow(Gtk.ApplicationWindow):
    def __init__(self, application):
        super().__init__(application=application, title="Barkditor")
        self.set_size_request(900, 700)
        self.set_show_menubar(True)
        self.set_menubar()

        main_box = Gtk.Box()
        main_box.set_orientation(Gtk.Orientation(1))
        main_box.set_visible(True)
        self.set_child(main_box)

    def set_menubar(self):
        # https://docs.gtk.org/gio/class.Menu.html
        # https://docs.gtk.org/gio/class.MenuItem.html
        # https://docs.gtk.org/gtk3/class.Application.html

        menubar = Gio.Menu()

        # Files Menu
        file_submenu_item = Gio.MenuItem()
        file_submenu_item.set_label("_Files")
        file_submenu = Gio.Menu()
        file_submenu_item.set_submenu(file_submenu)

        open_file_menu_item = Gio.MenuItem()
        open_file_menu_item.set_label("_Open File")
        file_submenu.append_item(open_file_menu_item)

        open_folder_menu_item = Gio.MenuItem()
        open_folder_menu_item.set_label("_Open Folder")
        file_submenu.append_item(open_folder_menu_item)

        save_menu_item = Gio.MenuItem()
        save_menu_item.set_label("_Save")
        file_submenu.append_item(save_menu_item)

        save_as_menu_item = Gio.MenuItem()
        save_as_menu_item.set_label("_Save As")
        file_submenu.append_item(save_as_menu_item)

        exit_menu_item = Gio.MenuItem()
        exit_menu_item.set_label("_Exit")
        file_submenu.append_item(exit_menu_item)

        # Help Menu
        help_submenu_item = Gio.MenuItem()
        help_submenu_item.set_label("_Help")
        help_submenu = Gio.Menu()
        help_submenu_item.set_submenu(help_submenu)

        about_menu_item = Gio.MenuItem()
        about_menu_item.set_label("_About Barkditor")
        help_submenu.append_item(about_menu_item)

        menubar.append_item(file_submenu_item)
        menubar.append_item(help_submenu_item)

        self.get_application().set_menubar(menubar=menubar)
