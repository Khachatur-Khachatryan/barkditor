from PyQt6.QtWidgets import QMainWindow, QTreeView, QWidget, \
    QHBoxLayout, QPlainTextEdit, QSplitter


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Barkditor")
        self.setMinimumSize(600, 600)

        menu_bar = self.menuBar()

        file_menu = menu_bar.addMenu("Files")
        open_file_action = file_menu.addAction("Open File")
        # open_file_action.triggered.connect(self.test_triggered)

        open_folder_action = file_menu.addAction("Open Folder")

        save_action = file_menu.addAction("Save")
        save_as_action = file_menu.addAction("Save as")
        file_menu.addSeparator()
        exit_action = file_menu.addAction("Exit")

        help_menu = menu_bar.addMenu("Help")
        about_action = help_menu.addAction("About Barkditor")

        self.init_main_box()

    def init_main_box(self):
        main_vbox = QHBoxLayout()
        splitter = QSplitter()
        tree_view = QTreeView()
        code_view = QPlainTextEdit()
        code_view.setMinimumWidth(200)

        splitter.addWidget(tree_view)
        splitter.addWidget(code_view)
        splitter.setCollapsible(1, False)

        main_vbox.addWidget(splitter)
        window = QWidget()

        window.setLayout(main_vbox)
        window.show()
        self.setCentralWidget(window)
