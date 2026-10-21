# DomForge Layered Web API Solution Template

## Requirements

- Windows with the .NET 10 SDK installed.
- Visual Studio 2026 with the ASP.NET and web development workload.

Administrator permissions are normally not required. The template is installed for the current Windows user.

## Install

1. Extract the entire ZIP file to a local folder. Do not run the installer from inside the ZIP preview.
2. Close every Visual Studio 2026 window.
3. Double-click `Install-Template.cmd`.
4. Open Visual Studio 2026 and select **Create a new project**.
5. Clear the language, platform, and project-type filters.
6. Search for **DomForge Layered Web API Solution** or **DomForge**.

Command-line installation is also supported:

```powershell
dotnet new install .\DomForge.WebApi.Template.1.0.4.nupkg --force
```

Create a project from the command line:

```powershell
dotnet new df-webapi -n MyCompany.MyApi
```

## Upgrade

Run `Install-Template.cmd` from the newer bundle. The installer removes duplicate registrations, installs the current package, and refreshes the Visual Studio 2026 template cache.

## Uninstall

Double-click `Uninstall-Template.cmd`, or run:

```powershell
dotnet new uninstall DomForge.WebApi.Template
```

Restart Visual Studio after installing, upgrading, or uninstalling the template.

