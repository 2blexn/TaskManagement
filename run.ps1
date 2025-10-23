Write-Host "Starting Task Management API..." -ForegroundColor Green
Write-Host ""

Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore

Write-Host ""
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build

Write-Host ""
Write-Host "Starting application..." -ForegroundColor Green
Write-Host "API will be available at:" -ForegroundColor Cyan
Write-Host "  HTTP:  http://localhost:5288" -ForegroundColor White
Write-Host "  HTTPS: https://localhost:7235" -ForegroundColor White
Write-Host "  Swagger: https://localhost:7235/swagger" -ForegroundColor White
Write-Host ""

dotnet run
