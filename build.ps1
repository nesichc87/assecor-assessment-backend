Write-Host "=== BUILD PersonsApi ===" -ForegroundColor Cyan

$project = "src/PersonsApi/PersonsApi.csproj"

dotnet restore $project
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed"
    exit 1
}

dotnet build $project -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

Write-Host "Build successful ✔" -ForegroundColor Green