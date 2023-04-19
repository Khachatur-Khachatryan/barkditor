# Barkditor

<p align="center">
    <img src="./img/barkditor-logo.svg" width="150" height="150" alt="Barkditor logo">
</p>

[![.NET build and test](https://github.com/Khachatur-Khachatryan/barkditor/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/Khachatur-Khachatryan/barkditor/actions/workflows/build_and_test.yml)

## Description
Barkditor is a open source code editor that implemented using Gir.Core library and .NET platform.
In general, this project created for educational purposes.
The project is currently in very alpha.

## Build and run project locally

### Prerequisites

Obligatory required software:

- **For Linux-based systems:** install xsel utility
- **Blueprint compiler**: install blueprint-compiler package
- **Install Blueprint compiler for Windows**:
  - **Install MSYS2**: https://www.msys2.org/
  - **Run commands following below:**
    ```
    pacman -Suy
    pacman -S mingw-w64-x86_64-gtk4 mingw-w64-x86_64-python3 mingw-w64-x86_64-python3-gobject mingw-w64-x86_64-ninja
    ```
  - **Clone blueprint-compiler source code:**
    ```
    git clone https://gitlab.gnome.org/jwestman/blueprint-compiler.git
    ```
  - **Build:**
    ```
    ninja -C _build install
    ```
- **Install Blueprint compiler for MacOS**
  - **Install Python3:** https://www.python.org/downloads/macos/
  - **Run commands following below:**
    ```
    brew install pygobject3 gtk4
    python3 -m pip install ninja
    ```
  - **Clone blueprint-compiler source code:**
    ```
    git clone https://gitlab.gnome.org/jwestman/blueprint-compiler.git
    ```
  - **Build:**
    ```
    ninja -C _build install
    ```
- **Install Blueprint compiler for DebianL**
  - ```
    sudo apt install blueprint-compiler
    ```
- **GTK library 4:** https://www.gtk.org/
- **.NET SDK 7.0.200 or later:** https://dotnet.microsoft.com/en-us/download
- **Code Editor or IDE:** Visual studio, Visual studio code, Rider

### Run

- **Build solution:** `dotnet build`
- **Go to server directory**: `cd ./src/server`
- **Run server**: `dotnet run --project BarkditorServer.Presentation`
- **Open new terminal and go to client directory**: `cd ./src/client`
- **Run client**: `dotnet run --project BarlditorGui.Presentation`

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
- **GTK:** `4`

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
