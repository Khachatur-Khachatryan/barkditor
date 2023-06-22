from PyQt6.QtGui import QFont
from PyQt6.QtWidgets import QApplication
from main_window import MainWindow


def start():
    app = QApplication([])

    font = QFont("Arial", 14)
    app.setFont(font)

    window = MainWindow()
    window.show()

    app.exec()


# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    start()
