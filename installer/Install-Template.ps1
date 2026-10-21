[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$packageId = 'DomForge.WebApi.Template'
$shortName = 'df-webapi'
$packageFiles = @(Get-ChildItem -LiteralPath $PSScriptRoot -File | Where-Object { $_.Name -like 'DomForge.WebApi.Template.*.nupkg' })

function Invoke-DotNet {
    param([Parameter(Mandatory)][string[]] $Arguments)

    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'The .NET SDK was not found. Install the .NET 10 SDK and run this installer again.'
}

if ($packageFiles.Count -ne 1) {
    throw "Expected exactly one DomForge template package next to the installer, but found $($packageFiles.Count)."
}

$packagePath = $packageFiles[0].FullName

$installedSdks = @(& dotnet --list-sdks)
if ($LASTEXITCODE -ne 0) {
    throw 'Could not read the installed .NET SDKs.'
}

if (-not ($installedSdks | Where-Object { $_ -match '^10\.' })) {
    throw 'The .NET 10 SDK is required. Install it and run this installer again.'
}

Write-Host "Removing older registrations of $packageId..." -ForegroundColor Cyan
$installedPackages = (& dotnet new uninstall 2>&1 | Out-String)
if ($LASTEXITCODE -ne 0) {
    throw 'Could not read the installed template packages.'
}

if ($installedPackages -match [regex]::Escape($packageId)) {
    Invoke-DotNet -Arguments @('new', 'uninstall', $packageId)
}

Write-Host "Installing $packageId..." -ForegroundColor Cyan
Invoke-DotNet -Arguments @('new', 'install', $packagePath, '--force')

Write-Host "Verifying template '$shortName'..." -ForegroundColor Cyan
Invoke-DotNet -Arguments @('new', 'list', $shortName)

$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (Test-Path -LiteralPath $vswhere -PathType Leaf) {
    $vs2026Installations = @(& $vswhere -products * -version '[18.0,19.0)' -property installationPath)
    foreach ($installationPath in $vs2026Installations) {
        if ([string]::IsNullOrWhiteSpace($installationPath)) {
            continue
        }

        $devenv = Join-Path $installationPath 'Common7\IDE\devenv.com'
        if (-not (Test-Path -LiteralPath $devenv -PathType Leaf)) {
            continue
        }

        $isRunning = $false
        try {
            $isRunning = [bool](Get-CimInstance Win32_Process -Filter "Name='devenv.exe'" |
                Where-Object { $_.ExecutablePath -and $_.ExecutablePath.StartsWith($installationPath, [System.StringComparison]::OrdinalIgnoreCase) })
        }
        catch {
            Write-Warning "Could not determine whether Visual Studio 2026 is running: $($_.Exception.Message)"
        }

        if ($isRunning) {
            Write-Warning 'Visual Studio 2026 is running. Close and reopen it so the template cache can refresh.'
            continue
        }

        Write-Host 'Refreshing the Visual Studio 2026 template cache...' -ForegroundColor Cyan
        & $devenv /UpdateConfiguration
        if ($LASTEXITCODE -ne 0) {
            Write-Warning 'Visual Studio cache refresh did not complete. Restart Visual Studio 2026 before searching for the template.'
        }
    }
}

Write-Host ''
Write-Host 'Installation completed successfully.' -ForegroundColor Green
Write-Host 'Open Visual Studio 2026, choose Create a new project, clear all filters, and search for:'
Write-Host '  DomForge Layered Web API Solution' -ForegroundColor Yellow
Write-Host ''
Write-Host 'CLI example:'
Write-Host '  dotnet new df-webapi -n MyCompany.MyApi'

