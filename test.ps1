Write-Host "=== RUN TESTS ===" -ForegroundColor Cyan

$testProject = "PersonsApi.Tests/PersonsApi.Tests.csproj"

dotnet test $testProject -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "Tests failed"
    exit 1
}

Write-Host "All tests passed ✔" -ForegroundColor Green