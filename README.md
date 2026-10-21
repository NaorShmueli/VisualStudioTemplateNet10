# DomForge Layered Web API Template

A reusable .NET 10 solution template for starting a layered ASP.NET Core Web
API with sensible production-oriented defaults.

The generated solution includes JWT issuing and validation, Google
authentication placeholders, health checks, Redis-backed data protection,
App.Metrics, Serilog, Swagger, Docker support, and xUnit tests.

## Prerequisites

- Visual Studio 2026 with the **ASP.NET and web development** workload.
- .NET 10 SDK.
- Git for cloning the repository.

## Fast installation

Clone the repository, close Visual Studio 2026, and run the installer:

```powershell
git clone https://github.com/NaorShmueli/VisualStudioTemplateNet10.git
cd VisualStudioTemplateNet10
.\installer\Install-Template.cmd
```

Reopen Visual Studio, select **Create a new project**, and search for
**DomForge Layered Web API Solution**.

Create a solution directly from the command line when preferred:

```powershell
dotnet new df-webapi -n Orders.Api
```

The generated solution uses the Visual Studio 2026 `.slnx` format.

## Generated structure

```text
Abstraction/
BusinessLogic/
DAL/
HttpManager/
Models/
Validator/
YourApi/
YourApi.UnitTests/
SolutionItems/
YourApi.slnx
```

See the generated `ARCHITECTURE.md` for assembly responsibilities and
dependency rules.

## Develop and validate the template

The source solution under `content/DfWebApiSolution` is buildable and testable:

```powershell
dotnet build .\content\DfWebApiSolution\WebApiTemplate.slnx -c Release
dotnet test .\content\DfWebApiSolution\WebApiTemplate.slnx -c Release
dotnet pack .\DomForge.WebApi.TemplatePack.csproj -c Release -o .\artifacts
```

After changing the template, update the package under `installer/` before
publishing a new version.

## Uninstall

```powershell
dotnet new uninstall DomForge.WebApi.Template
```

For a short interview-day checklist and fallback commands, see
[INTERVIEW-SETUP.md](INTERVIEW-SETUP.md).
