import grpc
from PyQt6.QtWidgets import QMenuBar, QTreeView, QFileDialog, QPlainTextEdit, QMessageBox, QApplication
from PyQt6.QtGui import QAction
from PyQt6.QtCore import QDir
from grpc_services import files_client, project_files_client
from Protos import files_pb2, projectFiles_pb2
from pathlib import Path
import contextlib


class MenuBar(QMenuBar):
    """Barkditor top menubar"""
    def __init__(self, file_tree_view: QTreeView, code_edit: QPlainTextEdit):
        """Default constructor"""
        super().__init__()
        # Initialize private fields
        self.__file_tree_view = file_tree_view
        self.__code_edit = code_edit

        # "Files" submenu setup
        file_menu = self.addMenu("Files")
        open_file_action = file_menu.addAction("Open File")
        open_file_action.triggered.connect(self.open_file)
        open_folder_action = file_menu.addAction("Open Folder")
        open_folder_action.triggered.connect(self.open_folder)

        # TODO: BARKDITOR-GUI-73, BARKDITOR-SERVER-49
        save_action = file_menu.addAction("Save")

        # TODO: BARKDITOR-GUI-74, BARKDITOR-SERVER-50
        save_as_action = file_menu.addAction("Save as")

        file_menu.addSeparator()
        exit_action = file_menu.addAction("Exit")
        exit_action.triggered.connect(self.exit)

        # "Help" submenu setup
        help_menu = self.addMenu("Help")
        about_action = help_menu.addAction("About Barkditor")

    def open_file(self) -> None:
        dialog_response = QFileDialog.getOpenFileName(parent=self, caption="Open File", directory=str(Path.home()))
        with contextlib.suppress(Exception):
            request = files_pb2.GetFileContentRequest(path=dialog_response[0])
            response = files_client.GetFileContent(request)
            self.__code_edit.setPlainText(response.content)

    def open_folder(self) -> None:
        dialog_response = QFileDialog.getExistingDirectory(parent=self, caption="Open Folder", directory=str(Path.home()))

        if dialog_response == '':
            return

        try:
            request = projectFiles_pb2.SetProjectPathRequest(path="dialog_response")
            project_files_client.SetProjectPath(request)
        except grpc.RpcError as error:
            QMessageBox.critical(self, "Error", error.details())
            return

        model = self.__file_tree_view.model()
        model.setRootPath(dialog_response)
        self.__file_tree_view.setRootIndex(model.index(dialog_response))

    def exit(self) -> None:
        QApplication.quit()
