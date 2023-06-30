from PyQt6.QtWidgets import QDialog, QWidget, QDialogButtonBox, QVBoxLayout, QHBoxLayout, QTabWidget, QLabel, \
    QScrollArea, QSizePolicy
from PyQt6.QtGui import QPainter, QPixmap
from PyQt6.QtCore import Qt
from PyQt6.QtSvg import QSvgRenderer


class AboutDialog(QDialog):
    def __init__(self, parent: QWidget):
        super().__init__(parent)
        self.setWindowTitle("About Barkditor")
        self.setBaseSize(900, 900)
        self.setMinimumSize(500, 500)
        size_policy = QSizePolicy(QSizePolicy.Policy.Minimum, QSizePolicy.Policy.Minimum)
        self.setSizePolicy(size_policy)
        self.init_layout()

    def init_layout(self):
        # App info widget setup
        app_info_widget = QWidget()
        app_info_layout = QHBoxLayout()
        logo_label = QLabel()
        svg_renderer = QSvgRenderer("../../img/barkditor-logo.svg")
        pixmap = QPixmap(64, 64)
        pixmap.fill(Qt.GlobalColor.transparent)
        painter = QPainter(pixmap)
        svg_renderer.render(painter)
        painter.setCompositionMode(painter.CompositionMode.CompositionMode_SourceIn)
        painter.end()
        logo_label.setPixmap(pixmap)
        app_info_label = QLabel("Barkditor \n Version: 0.0.0-alpha")
        app_info_layout.setContentsMargins(0, 0, 0, 0)
        app_info_layout.addWidget(logo_label)
        app_info_layout.addWidget(app_info_label)
        app_info_layout.addStretch()
        app_info_widget.setLayout(app_info_layout)


        # About application page setup
        about_page = QWidget()
        about_page_layout = QVBoxLayout()
        description_label = QLabel("<p>Barkditor is an open source code editor that implemented using PyQt6 framework "
                                   "and .NET 7 platform. In general, this project created for educational purposes. The"
                                   " project is currently in very alpha.</p>"
                                   "<br/>"
                                   "<a href=\"https://github.com/Khachatur-Khachatryan/barkditor\">Source Code</a>")
        description_label.setOpenExternalLinks(True)
        description_label.setWordWrap(True)
        description_label.setTextInteractionFlags(Qt.TextInteractionFlag.TextSelectableByMouse)
        about_page_layout.addWidget(description_label)
        about_page_layout.addStretch()
        about_page.setLayout(about_page_layout)

        # Authors page setup
        authors_page = QWidget()
        authors_page_layout = QVBoxLayout()
        authors_label = QLabel("Khachatur Khachatryan: main developer")
        authors_label.setWordWrap(True)
        authors_label.setTextInteractionFlags(Qt.TextInteractionFlag.TextSelectableByMouse)
        authors_label.setAlignment(Qt.AlignmentFlag.AlignTop)
        authors_page_layout.addWidget(authors_label)
        about_page_layout.addStretch()
        authors_page.setLayout(authors_page_layout)

        # License page setup
        license_page = QScrollArea()
        license_label = QLabel()
        license_label.setTextInteractionFlags(Qt.TextInteractionFlag.TextSelectableByMouse)
        with open("../../LICENSE", mode="r") as license_file:
            license_text = license_file.read()
            license_label.setText(license_text)
        license_page.setWidget(license_label)

        # Tab widget setup
        tab_widget = QTabWidget()
        tab_widget.addTab(about_page, "About")
        tab_widget.addTab(authors_page, "Authors")
        tab_widget.addTab(license_page, "License")

        # Button box setup
        cancel_button = QDialogButtonBox.StandardButton.Cancel
        dialog_button_box = QDialogButtonBox(cancel_button)
        dialog_button_box.rejected.connect(lambda: self.close())

        # Layout setup
        layout = QVBoxLayout()
        layout.addWidget(app_info_widget)
        layout.addWidget(tab_widget)
        layout.addWidget(dialog_button_box)
        self.setLayout(layout)
