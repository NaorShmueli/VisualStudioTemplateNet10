# Interview setup checklist

## Before the interview

1. Push this repository and confirm it can be cloned from a clean directory.
2. Keep a second copy of `installer/` on a USB drive or private cloud folder in
   case access to GitHub is restricted.
3. Confirm that using a personal starter template is acceptable for the
   interview exercise.
4. Ask whether the supplied computer has Visual Studio 2026, the ASP.NET and
   web development workload, and the .NET 10 SDK.
5. Keep this repository URL ready to paste.

## On the interview computer

Verify the SDK first:

```powershell
dotnet --list-sdks
```

The output must contain a `10.0` SDK. Then clone and install:

```powershell
git clone <repository-url>
cd WebApiSolutionTemplate
.\installer\Install-Template.cmd
```

Open Visual Studio 2026, choose **Create a new project**, and search for
**DomForge Layered Web API Solution**.

## Command-line fallback

If the Visual Studio template dialog is slow, create the solution directly:

```powershell
dotnet new df-webapi -n Interview.Api -o .\Interview.Api
cd .\Interview.Api
dotnet build .\Interview.Api.slnx
dotnet test .\Interview.Api.slnx
```

Open `Interview.Api.slnx` in Visual Studio 2026.

## Remove the template afterward

```powershell
dotnet new uninstall DomForge.WebApi.Template
```
