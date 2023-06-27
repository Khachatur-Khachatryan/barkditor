from PyQt6.QtGui import QFileSystemModel
from PyQt6.QtWidgets import QMainWindow, QTreeView, QWidget, \
    QHBoxLayout, QVBoxLayout, QPlainTextEdit, QSplitter
from menubar import MenuBar


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()

        # Initialize private fields
        self.__tree_view = QTreeView()
        self.__code_edit = QPlainTextEdit()

        # Configure main window
        self.setWindowTitle("Barkditor")
        self.setMinimumSize(900, 600)

        # Widget initialization
        self.init_file_tree_view()
        menubar = MenuBar(self.__tree_view, self.__code_edit)
        self.setMenuBar(menubar)
        self.init_main_layout()

    def init_main_layout(self) -> None:
        """Adds main layout to window and adds tree view and text edit widget"""
        # Main layouts
        main_vbox = QHBoxLayout()
        splitter = QSplitter()

        # File viewer widget setup
        file_viewer_box = QVBoxLayout()
        file_viewer_box.addWidget(self.__tree_view)
        file_viewer_box.setContentsMargins(0, 0, 0, 0)
        file_viewer_widget = QWidget()
        file_viewer_widget.setLayout(file_viewer_box)

        # Splitter setup
        splitter.addWidget(file_viewer_widget)
        splitter.addWidget(self.__code_edit)
        splitter.setCollapsible(1, False)
        splitter.setSizes([220, 480])

        # Main VBox setup
        main_vbox.setContentsMargins(3, 3, 3, 3)
        main_vbox.addWidget(splitter)

        # Window setup
        window = QWidget()
        window.setLayout(main_vbox)
        window.show()
        self.setCentralWidget(window)

    def init_file_tree_view(self) -> None:
        """Initializes file tree view"""
        file_system_model = QFileSystemModel()
        self.__tree_view.setModel(file_system_model)
        self.__tree_view.setHeaderHidden(True)
        for i in range(1, file_system_model.columnCount()):
            self.__tree_view.hideColumn(i)
