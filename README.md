# Barkditor

<p align="center">
    <img src="./img/barkditor-logo.svg" width="150" height="150" alt="Barkditor logo">
</p>

[![.NET build and test](https://github.com/Khachatur-Khachatryan/barkditor/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/Khachatur-Khachatryan/barkditor/actions/workflows/build_and_test.yml)

## Description
Barkditor is a open source code editor that implemented using PyQt6 framework and .NET 7 platform.
In general, this project created for educational purposes.
The project is currently in very alpha.

## Build and run project locally

### Prerequisites

Obligatory required software:

- **For GNU/Linux-based systems:** install xsel utility (Xorg) / wl-clipboard (Wayland)
- **.NET SDK 7.0.200 or later:** https://dotnet.microsoft.com/en-us/download
- **Code Editor or IDE:** Visual Studio, Visual Studio Code, JetBrains Rider
- **Python3:** https://www.python.org/downloads/
- **PyQt6 framework:** https://pypi.org/project/PyQt6/

### Run

- **Build solution:** `dotnet build`
- **Go to server directory**: `cd ./src/server`
- **Run server**: `dotnet run --project BarkditorServer.Presentation`
- **Open new terminal and go to client directory**: `cd ./src/client`
- **Install required packages**: `pip install -r requirements.txt`
- **Generate gRPC services**: `python -m grpc.tools.protoc -I=../shared/Barkditor.Protobuf/ --grpc_python_out=./ Protos/files.proto Protos/projectFiles.proto`
- **Run client**: `python __main__.py`

### Run unit tests

- **Build project:** `dotnet build`
- **Go to unit-test project:** `cd BarkditorServer.UnitTests`
- **Run tests:** `dotnet test`

## Technology stack

- **SDK:** `.NET 7`
- **Frameworks:** `ASP.NET`, `GtkSharp`
- **Unit testing:** `XUnit`, `FluentAssertions`
- **Programming languages:** `C#`
- **Tools & IDE:** `Visual Studio`, `Rider`, `VS Code`

## Versions

- **.NET:** `7.0.200 or later`
- **PyQt:** `6.5.1 or later`

## Tasks management

### For server

The opened tasks and issues to be organized an handled as follows:

- Each task has an assigned number in the format `BARKDITOR-SERVER-ID`
- Active tasks are available on the Trello board: https://trello.com/b/97i3UXAf/brakditor-server
- Each task branch is based on the actual `develop` branch and pull requested there on complete

### For client

The opened tasks and issues to be organized an handled as follows:

- Each task has an assigned number in the format `BARKDITOR-GUI-ID`
- Active tasks are available on the Trello board: https://trello.com/b/OW3eMcPr/barkditor-gtk-client
- Each task branch is based on the actual `develop` branch and pull requested there on complete


## Git flow

Version control to be organized as follows:

- Fork this repository
- Clone this repository using `git clone https://github.com/${{ username }}/barkditor.git`
- If repository is cloned already then pull last changes from `develop` using
    - `git checkout develop`
    - `git pull`
- Create new branch based on `develop` with name according to `BARKDITOR-SERVER-ID` of the task
- Solve the task
- Create pull request to `develop`

## Commit messages

- In case of bug fix, example of commit message `bugfix: some bug fixed`
- In case of feature, example of commit message `feature: some new functionality added`
- In case of refactor, example of commit message `refactor: some code part refactored`

## Logo

The logo was created in Inkscape.