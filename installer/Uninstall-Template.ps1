[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$packageId = 'DomForge.WebApi.Template'

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'The .NET SDK was not found.'
}

Write-Host "Uninstalling $packageId..." -ForegroundColor Cyan
& dotnet new uninstall $packageId
if ($LASTEXITCODE -ne 0) {
    throw "Could not uninstall $packageId. It may not be installed for the current Windows user."
}

Write-Host 'The template was removed successfully.' -ForegroundColor Green
Write-Host 'Restart Visual Studio if it is currently open.'

