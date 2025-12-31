Write-Host "=== BUILD & TEST ===" -ForegroundColor Cyan

.\build.ps1
if ($LASTEXITCODE -ne 0) {
    exit 1
}

.\test.ps1
if ($LASTEXITCODE -ne 0) {
    exit 1
}

Write-Host "Build & Test successful 🎉" -ForegroundColor Green