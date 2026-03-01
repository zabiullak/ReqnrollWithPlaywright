$solutionRoot  = Split-Path -Parent $MyInvocation.MyCommand.Path
$project       = Join-Path $solutionRoot "ReqnrollWithPlaywright\ReqnrollWithPlaywright.csproj"
$allureResults = Join-Path $solutionRoot "ReqnrollWithPlaywright\bin\Debug\net10.0\allure-results"
$testReport    = Join-Path $solutionRoot "TestReport"

# Clean previous results and report
if (Test-Path $allureResults) { Remove-Item -Recurse -Force $allureResults }
if (Test-Path $testReport)    { Remove-Item -Recurse -Force $testReport    }

# Run tests
Write-Host "`n==> Running tests...`n" -ForegroundColor Cyan
dotnet test $project --configuration Debug

# Generate Allure HTML report
Write-Host "`n==> Generating Allure HTML report...`n" -ForegroundColor Cyan
allure generate $allureResults --clean -o $testReport

Write-Host "`n==> Report ready at: $testReport`n" -ForegroundColor Green

# Open the report in the default browser
allure open $testReport
