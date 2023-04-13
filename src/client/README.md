# Barkditor GUI

<p align="center">
    <img src="./img/barkditor-logo.svg" width="150" height="150" alt="Barkditor logo">
</p>

## Description
Barkditor is a open source code editor that implemented using GTK Sharp lib and .NET framework.
In general, this project created for educational purposes.
The project is currently under development.

## Build and run project locally

### Prerequisites

Obligatory required software:

- **.NET SDK 7.0.200 or later:** https://dotnet.microsoft.com/en-us/download
- **GTK library 3.24:** https://www.gtk.org/
- **Code Editor or IDE:** Visual studio, Visual studio code, Rider

### Run

- **Run server by https://github.com/barkditor/barkditor-server guide**
- **Build project:** `dotnet build`
- **Run project:** `dotnet run -p BarkditorGui.Presentation`

## Technology stack

- **SDK:** `.NET 6`
- **Frameworks:** `ASP.NET`, `GtkSharp`
- **Unit testing:** `XUnit`, `FluentAssertions`\
- **Programming languages:** `C#`
- **Tools & IDE:** `Visual Studio`, `Rider`, `VS Code`

## Versions

- **.NET:** `7.0.200 or later`
- **GTK:** `3.24`

## Tasks management

The opened tasks and issues to be organized an handled as follows:

- Each task has an assigned number in the format `BARKDITOR-GUI-ID`
- Active tasks are available on the Trello board: https://trello.com/b/OW3eMcPr/barkditor-gtk-client
- Each task branch is based on the actual `develop` branch and pull requested there on complete

## Git flow

Version control to be organized as follows:

- Fork this repository
- Clone this repository using `git clone https://github.com/${{ username }}/barkditor-gui.git`
- If repository is cloned already then pull last changes from `develop` using
    - `git checkout develop`
    - `git pull`
- Create new branch based on `develop` with name according to `BARKDITOR-GUI-ID` of the task
- Solve the task
- Create pull request to `develop`

## Commit messages

- In case of bug fix, example of commit message `bugfix: some bug fixed`
- In case of feature, example of commit message `feature: some new functionality added`
- In case of refactor, example of commit message `refactor: some code part refactored`

## Logo

Logo created by Inkspace program.
