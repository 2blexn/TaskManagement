# Task Management API - Quick Start

## Quick Launch

### Option 1: Command Line
```bash
dotnet restore
dotnet build
dotnet run
```

### Option 2: Scripts
- **Windows**: Double-click `run.bat`
- **PowerShell**: Run `.\run.ps1`

### Option 3: Visual Studio
1. Open `TaskManagement.sln`
2. Press F5 or click the "Start" button

## API Access

- **HTTP**: http://localhost:5288
- **HTTPS**: https://localhost:7235
- **Swagger UI**: https://localhost:7235/swagger

## Test Data

**Administrator:**
- Username: `admin`
- Password: `admin123`
- Email: `admin@taskmanagement.com`

## Database

The API will automatically create the database on first run using SQL Server LocalDB.

## API Testing

Use the `TaskManagement.http` file to test endpoints in Visual Studio Code with the REST Client extension.
