# Task Management API

RESTful API for managing tasks built with .NET 8, Entity Framework Core, and JWT authentication. 

## Features

- **User Authentication & Authorization**: JWT-based authentication system
- **Task Management**: Full CRUD operations for tasks
- **Advanced Filtering**: Filter tasks by status, priority, due date, and search terms
- **Pagination**: Efficient data retrieval with pagination support
- **Data Validation**: Comprehensive input validation using FluentValidation
- **AutoMapper**: Clean data mapping between DTOs and entities
- **Repository Pattern**: Clean separation of data access logic
- **Service Layer**: Business logic encapsulation following SOLID principles
- **Swagger Documentation**: Interactive API documentation
- **Local Development**: Easy setup for local development
- **Database Migrations**: Entity Framework Core migrations

## Architecture

The project follows a layered architecture pattern:

```
├── Controllers/          # API Controllers
├── Services/            # Business Logic Layer
├── Repositories/        # Data Access Layer
├── Models/              # Domain Models
├── DTOs/                # Data Transfer Objects
├── Mappings/            # AutoMapper Profiles
├── Validators/          # FluentValidation Rules
└── Data/               # DbContext and Database Configuration
```

## Technology Stack

- **.NET 8**: Latest .NET framework
- **Entity Framework Core 8**: ORM for database operations
- **SQL Server**: Database engine
- **JWT Bearer Authentication**: Token-based authentication
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Input validation
- **Swagger/OpenAPI**: API documentation
- **SQL Server LocalDB**: Local database for development (automatically created)

## Getting Started

> **Quick Start**: For quick launch instructions see [QUICKSTART.md](QUICKSTART.md)

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code (recommended)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/2blexn/TaskManagement.git
   cd TaskManagement
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update connection string (if needed)**
   The default connection string uses SQL Server Express. If you're using a different SQL Server instance, update `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "your-connection-string-here"
     }
   }
   ```

4. **Restore packages and build**
   ```bash
   dotnet restore
   dotnet build
   ```

5. **Run the application**
   
   **Option 1: Using command line**
   ```bash
   dotnet run
   ```
   
   **Option 2: Using provided scripts**
   - Windows Batch: Double-click `run.bat`
   - PowerShell: Run `.\run.ps1` in PowerShell
   
   **Option 3: Using Visual Studio**
   - Press F5 or click the "Start" button

6. **Access Swagger UI**
   Navigate to `http://localhost:5288` to view the interactive API documentation.

### Running with Visual Studio

1. Open `TaskManagement.sln` in Visual Studio 2022
2. Select the `https` profile in the debug dropdown
3. Press F5 or click the "Start" button
4. The application will automatically open in your browser with Swagger UI

## API Endpoints

### Authentication (`/api/auth`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/register` | Register new user | No |
| POST | `/login` | User login | No |
| GET | `/me` | Get current user info | Yes |
| PUT | `/me` | Update current user | Yes |
| POST | `/change-password` | Change password | Yes |

### Tasks (`/api/tasks`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | Get all user tasks | Yes |
| GET | `/filtered` | Get filtered tasks | Yes |
| GET | `/{id}` | Get task by ID | Yes |
| POST | `/` | Create new task | Yes |
| PUT | `/{id}` | Update task | Yes |
| DELETE | `/{id}` | Delete task | Yes |
| POST | `/{id}/complete` | Complete task | Yes |
| GET | `/overdue` | Get overdue tasks | Yes |
| GET | `/status/{status}` | Get tasks by status | Yes |

## Data Models

### User
- `Id`: Primary key
- `Username`: Unique username
- `Email`: Unique email address
- `PasswordHash`: Hashed password
- `FirstName`: User's first name
- `LastName`: User's last name
- `CreatedAt`: Creation timestamp
- `UpdatedAt`: Last update timestamp
- `IsActive`: Account status

### TaskItem
- `Id`: Primary key
- `Title`: Task title
- `Description`: Task description
- `Priority`: Task priority (Low, Medium, High, Critical)
- `Status`: Task status (Pending, InProgress, Completed, Cancelled)
- `CreatedAt`: Creation timestamp
- `UpdatedAt`: Last update timestamp
- `DueDate`: Task due date
- `CompletedAt`: Completion timestamp
- `UserId`: Foreign key to User

## Authentication

The API uses JWT (JSON Web Token) for authentication. To access protected endpoints:

1. Register a new user or login with existing credentials
2. Copy the JWT token from the response
3. Include the token in the Authorization header: `Bearer <your-token>`

### Default Admin User
- **Username**: `admin`
- **Password**: `admin123`
- **Email**: `admin@taskmanagement.com`

## Filtering and Pagination

### Task Filtering
Tasks can be filtered using query parameters:
- `status`: Filter by task status (1=Pending, 2=InProgress, 3=Completed, 4=Cancelled)
- `priority`: Filter by priority (1=Low, 2=Medium, 3=High, 4=Critical)
- `dueDateFrom`: Filter tasks due after this date
- `dueDateTo`: Filter tasks due before this date
- `searchTerm`: Search in title and description
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 10, max: 100)

### Example Filter Request
```
GET /api/tasks/filtered?status=1&priority=2&page=1&pageSize=10&searchTerm=urgent
```

## Error Handling

The API returns appropriate HTTP status codes and error messages:

- `200 OK`: Successful request
- `201 Created`: Resource created successfully
- `400 Bad Request`: Invalid input data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Access denied
- `404 Not Found`: Resource not found
- `409 Conflict`: Resource already exists
- `500 Internal Server Error`: Server error

## Development

### Adding New Features

1. **Create Domain Model**: Add new entity in `Models/`
2. **Create DTOs**: Add request/response DTOs in `DTOs/`
3. **Create Repository**: Implement data access in `Repositories/`
4. **Create Service**: Implement business logic in `Services/`
5. **Create Controller**: Add API endpoints in `Controllers/`
6. **Add Validation**: Create validators in `Validators/`
7. **Update Mappings**: Add AutoMapper profiles in `Mappings/`

### Database Setup

The application uses Entity Framework Core with automatic database creation. The database will be automatically created when you first run the application.

**Important**: The application uses `EnsureCreated()` which creates the database schema directly based on the current model. This approach bypasses migrations and creates the database from scratch each time.

**Note**: Current implementation uses `EnsureCreated()` for simplicity. For production environments, consider switching to `Migrate()` for proper migration support.
