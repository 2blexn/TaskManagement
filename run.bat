@echo off
echo Starting Task Management API...
echo.
echo Restoring packages...
dotnet restore
echo.
echo Building project...
dotnet build
echo.
echo Starting application...
echo API will be available at:
echo   HTTP:  http://localhost:5288
echo   HTTPS: https://localhost:7235
echo   Swagger: https://localhost:7235/swagger
echo.
dotnet run
pause
