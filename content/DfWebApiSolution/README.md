# WebApiTemplate

A reusable, layered ASP.NET Core Web API starter solution targeting .NET 10.
It starts without SQL Server, Redis, Google credentials, or JWT keys. Optional
infrastructure is enabled only when its configuration is present.

## Projects

See [ARCHITECTURE.md](ARCHITECTURE.md) for the responsibility and dependency
rules of every assembly.

## Run locally

```powershell
dotnet restore .\WebApiTemplate.slnx
dotnet run --project .\WebApiTemplate\WebApiTemplate.csproj
```

Open `http://localhost:5080/swagger` or check
`http://localhost:5080/health/live`.

## Enable local JWT issuing and validation

JWT is disabled by default so a freshly generated service can run without
secrets. Initialize a local RSA key pair in the development secret store:

```powershell
.\SolutionItems\Initialize-DevelopmentJwt.ps1
```

The script never writes keys into the repository. It enables JWT only in the
user-secrets store for this project. The API does not expose an anonymous token
endpoint: call `IJwtTokenService` only after your login or identity-provider
flow has authenticated the subject.

## Docker

```powershell
docker compose up --build
```

The API is available at `http://localhost:8080`. SQL Server and Redis are not
required. Provide their connection strings through environment variables when
the service needs them:

```text
ConnectionStrings__SqlServer=...
ConnectionStrings__Redis=...
```

Do not place credentials, signing keys, or production connection strings in
`appsettings.json`, `compose.yaml`, a Dockerfile, or source control.

## Tests

```powershell
dotnet test .\WebApiTemplate.slnx --collect:"XPlat Code Coverage"
```

The test project includes reusable JWT behavior tests.
