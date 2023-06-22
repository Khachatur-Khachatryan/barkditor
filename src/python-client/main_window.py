from PyQt6.QtGui import QFileSystemModel
from PyQt6.QtWidgets import QMainWindow, QTreeView, QWidget, \
    QHBoxLayout, QVBoxLayout, QPlainTextEdit, QSplitter


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()

        # initialize private fields
        self.__tree_view = QTreeView()
        self.__file_system_model = QFileSystemModel()

        # configure main window
        self.setWindowTitle("Barkditor")
        self.setMinimumSize(900, 600)

        # widget initialization
        self.init_menubar()
        self.init_main_layout()
        self.init_file_tree_view()

    def init_main_layout(self):
        """Adds main layout to window and adds tree view and text edit widget"""
        # Main layouts
        main_vbox = QHBoxLayout()
        splitter = QSplitter()

        # Code editor setup
        code_editor = QPlainTextEdit()
        code_editor.setMinimumWidth(200)

        # File viewer widget setup
        file_viewer_box = QVBoxLayout()
        file_viewer_box.addWidget(self.__tree_view)
        file_viewer_box.setContentsMargins(0, 0, 0, 0)
        file_viewer_widget = QWidget()
        file_viewer_widget.setLayout(file_viewer_box)

        # Splitter setup
        splitter.addWidget(file_viewer_widget)
        splitter.addWidget(code_editor)
        splitter.setCollapsible(1, False)
        splitter.setSizes([220, 380])

        # Main VBox setup
        main_vbox.setContentsMargins(3, 3, 3, 3)
        main_vbox.addWidget(splitter)

        # Window setup
        window = QWidget()
        window.setLayout(main_vbox)
        window.show()
        self.setCentralWidget(window)

    def init_menubar(self):
        """Initializes menubar actions and submenus"""
        menu_bar = self.menuBar()

        # "Files" submenu setup
        file_menu = menu_bar.addMenu("Files")
        open_file_action = file_menu.addAction("Open File")
        # open_file_action.triggered.connect(self.test_triggered)
        open_folder_action = file_menu.addAction("Open Folder")
        save_action = file_menu.addAction("Save")
        save_as_action = file_menu.addAction("Save as")
        file_menu.addSeparator()
        exit_action = file_menu.addAction("Exit")

        # "Help" submenu setup
        help_menu = menu_bar.addMenu("Help")
        about_action = help_menu.addAction("About Barkditor")

        # TODO: add event handlers for menu actions

    def init_file_tree_view(self):
        """Initializes file tree view"""
        test_path = "/home/khachatur/code/"
        self.__file_system_model.setRootPath(test_path)
        self.__tree_view.setHeaderHidden(True)
        self.__tree_view.setModel(self.__file_system_model)
        self.__tree_view.setRootIndex(self.__file_system_model.index(test_path))
        for i in range(1, self.__file_system_model.columnCount()):
            self.__tree_view.hideColumn(i)
