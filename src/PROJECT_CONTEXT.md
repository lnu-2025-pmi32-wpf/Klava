# PROJECT_CONTEXT.md

## 1. Project Overview

**Klava** is a Windows desktop application designed for students to manage their academic workflow. It provides a centralized platform for organizing teams, subjects, assignments, deadlines, and learning materials. The application enables team collaboration where users can create or join teams via invite codes, manage subjects with associated tasks, track submission statuses, and share files.

**Primary Use Case**: Students form teams (e.g., study groups), team leaders ("Headman") manage subjects and assignments, and all members can view tasks, submit work, and access shared materials.

---

## 2. Tech Stack

### Platform & Framework
- **.NET 9** (net9.0 / net9.0-windows)
- **WPF** (Windows Presentation Foundation) - Desktop UI framework
- **C#** with nullable reference types enabled

### Key Libraries & Packages

| Package | Version | Purpose |
|---------|---------|---------|
| CommunityToolkit.Mvvm | 8.4.0 | MVVM implementation (ObservableObject, RelayCommand) |
| Microsoft.EntityFrameworkCore | 9.0.0 | ORM for database access |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.2 | PostgreSQL EF Core provider |
| BCrypt.Net-Next | 4.0.3 | Password hashing |
| Microsoft.Extensions.DependencyInjection | 10.0.0 | DI container |
| Microsoft.Extensions.Hosting | 10.0.0 | Generic host for application lifecycle |
| Microsoft.Extensions.Configuration.Json | 10.0.0 | JSON configuration support |

### Testing
- **xUnit** (2.9.2) - Unit testing framework
- **Moq** (4.20.72) - Mocking library
- **Moq.EntityFrameworkCore** (8.0.2.1) - EF Core mocking extensions
- **Microsoft.EntityFrameworkCore.InMemory** (9.0.0) - In-memory database for tests

### Database
- **PostgreSQL 12+** - Primary database
- Custom enum types: `team_member_role`, `subject_status`, `submission_status`

### Tools
- **EF Core Migrations** - Database schema management
- **Docker** (optional) - PostgreSQL containerization

---

## 3. Project Structure

```
Klava/
├── src/
│   ├── Klava.sln                    # Solution file
│   │
│   ├── Klava.Domain/                # Domain Layer (Core)
│   │   ├── Entities/                # Domain entities (User, Team, Subject, Task, etc.)
│   │   └── Enums/                   # Enumerations (TeamMemberRole, SubjectStatus, SubmissionStatus)
│   │
│   ├── Klava.Infrastructure/        # Infrastructure Layer
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs      # EF Core DbContext
│   │   │   └── Configurations/      # Entity type configurations (Fluent API)
│   │   ├── Migrations/              # EF Core migrations
│   │   ├── Interfaces/              # Infrastructure contracts (IFileStorageService)
│   │   └── Services/                # Infrastructure services (FileStorageService)
│   │
│   ├── Klava.Application/           # Application Layer (Business Logic)
│   │   ├── DTOs/                    # Data Transfer Objects
│   │   └── Services/
│   │       ├── Interfaces/          # Service contracts
│   │       └── Implementations/     # Service implementations
│   │
│   ├── Klava.WPF/                   # Presentation Layer (WPF Desktop App)
│   │   ├── Views/                   # XAML views
│   │   ├── ViewModels/              # MVVM ViewModels
│   │   ├── Services/                # UI services (Navigation, Dialog, Session)
│   │   ├── Converters/              # XAML value converters
│   │   ├── App.xaml(.cs)            # Application entry point & DI configuration
│   │   ├── MainWindow.xaml(.cs)     # Main application window
│   │   └── appsettings.json         # Configuration (connection strings)
│   │
│   ├── Klava.Tests/                 # Unit Tests
│   │   └── Services/                # Service layer tests
│   │
│   ├── Klava.DataSeeder/            # CLI tool for database management
│   │   ├── Program.cs               # Entry point with menu system
│   │   ├── SeedData.cs              # Test data generation
│   │   └── Config.cs                # Database configuration
│   │
│   └── sql/
│       └── klava_database.sql       # Database schema script
│
└── Documents/                       # Project documentation
```

---

## 4. Key Features

### User Management
- User registration with BCrypt password hashing
- Authentication (login with firstname + lastname + password)
- Session management for logged-in user state

### Team Management
- Create teams with auto-generated unique invite codes
- Join teams via invite code
- Role-based access: **Student** and **Headman** (team leader)
- Promote/demote members between roles
- Remove members from teams

### Subject Management
- Create subjects within teams
- Subject status types: **Exam** or **Test**
- Subject info/description field
- File attachments per subject

### Task Management
- Create tasks with name, description, and optional deadline
- Tasks belong to subjects
- View tasks with user-specific submission status

### Submission System
- Students submit work for tasks
- Submission statuses: **Wait** (pending) and **Done** (completed)
- Track submission status per user per task

### File Management
- Upload files to subjects
- Local file storage service
- Download/access subject files

### Navigation & UI
- View-based navigation with parameter passing
- Dialog service for user prompts
- MVVM-based reactive UI updates

---

## 5. Architecture & Patterns

### Clean Architecture (Onion/Layered)
The project follows **Clean Architecture** principles with clear layer separation:

```
┌─────────────────────────────────────────────────────────────┐
│                    Klava.WPF (Presentation)                 │
│         Views, ViewModels, Navigation, Converters           │
└─────────────────────────┬───────────────────────────────────┘
                          │ depends on
┌─────────────────────────▼───────────────────────────────────┐
│                  Klava.Application (Business)               │
│              Services, DTOs, Business Rules                 │
└─────────────────────────┬───────────────────────────────────┘
                          │ depends on
┌─────────────────────────▼───────────────────────────────────┐
│                Klava.Infrastructure (Data Access)           │
│        DbContext, Migrations, Configurations, File I/O      │
└─────────────────────────┬───────────────────────────────────┘
                          │ depends on
┌─────────────────────────▼───────────────────────────────────┐
│                    Klava.Domain (Core)                      │
│                   Entities, Enums                           │
└─────────────────────────────────────────────────────────────┘
```

### Design Patterns Used

| Pattern | Implementation |
|---------|----------------|
| **MVVM** | ViewModels inherit from `ViewModelBase` (using CommunityToolkit.Mvvm), Views bind to ViewModels |
| **Dependency Injection** | Microsoft.Extensions.DependencyInjection configured in `App.xaml.cs` |
| **Repository Pattern** | Services encapsulate data access via `AppDbContext` |
| **Service Layer** | Business logic in `IXxxService`/`XxxService` pairs |
| **DTO Pattern** | Data transfer objects for cross-layer communication |
| **Navigation Service** | Centralized view navigation with parameter support |
| **Session Service** | Singleton for current user state management |

### Data Flow
1. **User Action** → View (XAML) triggers command
2. **ViewModel** → Executes command, calls Application Service
3. **Application Service** → Business logic, uses DbContext
4. **Infrastructure** → EF Core queries PostgreSQL
5. **Response** → Data flows back through layers, ViewModel updates properties
6. **UI Update** → Data binding updates View

---

## 6. Style & Conventions

### Naming Conventions
- **PascalCase**: Classes, methods, properties, public members
- **camelCase**: Local variables, parameters
- **_camelCase**: Private fields (with underscore prefix)
- **Async suffix**: All async methods (e.g., `GetTeamByIdAsync`)
- **I-prefix**: Interfaces (e.g., `ITeamService`)

### Project Conventions
- **File-scoped namespaces**: `namespace Klava.Domain.Entities;`
- **Nullable reference types enabled**: `<Nullable>enable</Nullable>`
- **Implicit usings enabled**: `<ImplicitUsings>enable</ImplicitUsings>`
- **Records/classes**: Entities as classes with navigation properties

### Entity Conventions
- `Id` property as primary key (int)
- Navigation properties initialized as `new List<T>()`
- Required properties use `= string.Empty` or `= null!`
- Optional properties use nullable types (`string?`, `int?`)

### Service Conventions
- Interface + Implementation pattern (`IXxxService` + `XxxService`)
- Services in `Services/Interfaces` and `Services/Implementations`
- Async by default with `Task<T>` returns
- Constructor injection for `AppDbContext`

### ViewModel Conventions
- Inherit from `ViewModelBase` (which inherits `ObservableObject`)
- Implement `INavigationAware` for parameter receiving
- Use `[ObservableProperty]` for bindable properties
- Use `[RelayCommand]` for commands

### XAML Conventions
- Views in `Views/` folder with `*View.xaml` naming
- Code-behind minimal (just `InitializeComponent()`)
- DataContext set via binding or navigation service

### Testing Conventions
- Test class per service: `XxxServiceTests.cs`
- In-memory database for integration tests
- Moq for dependency mocking
- xUnit with `[Fact]` attributes

### Configuration
- Connection string in `appsettings.json` under `ConnectionStrings:DefaultConnection`
- File storage path configurable (defaults to `uploads/` in app directory)

---

## Quick Reference

### Connection String
```
Host=localhost;Database=klava_db;Username=postgres;Password=admin
```

### Key Entities
- `User` (Id, Firstname, Lastname, Password)
- `Team` (Id, Name, Code, OwnerId)
- `TeamMember` (UserId, TeamId, Role)
- `Subject` (Id, TeamId, Title, SubjectInfo, Status)
- `Task` (Id, SubjectId, Name, Description, Deadline)
- `Submission` (Id, TaskId, UserId, Status)
- `SubjectFile` (Id, SubjectId, FileName, FilePath)

### Key Services
- `AuthService` - Registration/Login
- `TeamService` - Team CRUD, join by code
- `MemberService` - Member management, roles
- `SubjectService` - Subject CRUD
- `TaskService` - Task CRUD
- `SubmissionService` - Submission tracking
- `SubjectFileService` - File upload/download

### Running the App
```powershell
cd src
dotnet run --project .\Klava.WPF\Klava.WPF.csproj
```

### Running Tests
```powershell
cd src
dotnet test .\Klava.Tests\Klava.Tests.csproj
```
