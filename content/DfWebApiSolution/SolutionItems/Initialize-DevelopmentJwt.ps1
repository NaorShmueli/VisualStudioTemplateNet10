[CmdletBinding()]
param(
    [string]$ProjectPath = "$PSScriptRoot\..\WebApiTemplate\WebApiTemplate.csproj"
)

$resolvedProject = (Resolve-Path -LiteralPath $ProjectPath).Path
$rsa = [System.Security.Cryptography.RSA]::Create(3072)

try {
    $privateKey = [Convert]::ToBase64String($rsa.ExportPkcs8PrivateKey())
    $publicKey = [Convert]::ToBase64String($rsa.ExportSubjectPublicKeyInfo())

    & dotnet user-secrets set "Authentication:Jwt:Enabled" "true" --project $resolvedProject
    & dotnet user-secrets set "Authentication:Jwt:PrivateKey" $privateKey --project $resolvedProject
    & dotnet user-secrets set "Authentication:Jwt:PublicKey" $publicKey --project $resolvedProject

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet user-secrets failed with exit code $LASTEXITCODE."
    }

    Write-Host "Development JWT keys were stored in user-secrets."
}
finally {
    $rsa.Dispose()
}
