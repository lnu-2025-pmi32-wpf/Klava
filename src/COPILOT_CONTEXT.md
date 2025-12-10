# Klava - Student Task Management System
## AI Context File (Copilot Reference)

---

## 🎯 Project Overview
**Purpose**: Blazor Web App for students to manage teams, subjects, and academic tasks collaboratively  
**Status**: Phase 1 (Data Layer) & Phase 2 (Services) - Implementation Focus  
**Architecture**: Clean Architecture with MVVM pattern

---

## 🛠️ Tech Stack
- **.NET 9.0** (Blazor Web App - Interactive Server Mode)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core with Fluent API
- **Auth**: BCrypt.Net-Next (password hashing)
- **UI Pattern**: MVVM with DTOs

---

## 🏗️ Solution Structure & Dependencies

```
Klava.UI (Presentation)
    ↓ depends on
Klava.Application (Business Logic)
    ↓ depends on
Klava.Domain (Core Entities)
    ↑ referenced by
Klava.Infrastructure (Data Access)
```

### Project Responsibilities
- **Domain**: Pure entities/enums, no dependencies, no EF annotations
- **Infrastructure**: DbContext, EF Configurations (Fluent API), Migrations
- **Application**: Services (IService → Service), DTOs, business logic
- **UI**: Blazor components, ViewModels, SessionService

---

## 📊 Database Schema (PostgreSQL)

### Entities & Relationships
```
User (users)
├─ Id (PK)
├─ Firstname (string, required, max:100)
├─ Lastname (string, required, max:100)
└─ Password (hashed BCrypt, max:255)

Team (teams)
├─ Id (PK)
├─ Name (string, required, max:50)
├─ Code (string, unique, 8 chars, uppercase alphanumeric)
└─ OwnerId (FK → User, on delete: SET NULL)

TeamMember (teammembers) - Join Table
├─ TeamId (PK, FK → Team, cascade delete)
├─ UserId (PK, FK → User, cascade delete)
└─ Role (enum: student | headman)

Subject (subjects)
├─ Id (PK)
├─ TeamId (FK → Team, cascade delete)
├─ Title (string, required, max:255)
├─ SubjectInfo (text, nullable)
└─ Status (enum: exam | test)

Task (tasks)
├─ Id (PK)
├─ SubjectId (FK → Subject, cascade delete)
├─ Name (string, required, max:50)
├─ Description (text, nullable)
└─ Deadline (datetime, nullable)
```

### Enums
- **TeamMemberRole**: `Student`, `Headman`
- **SubjectStatus**: `Exam`, `Test`

### Key Relationships
- User 1:N TeamMember N:1 Team
- Team 1:N Subject 1:N Task
- Team.Owner → User (nullable, SET NULL on user delete)

---

## 🎨 Coding Standards & Patterns

### 1. Clean Architecture Rules
- ❌ **NO** EF Core attributes in Domain (use Fluent API in Infrastructure)
- ✅ Use separate `IEntityTypeConfiguration<T>` classes for each entity
- ✅ Apply configurations via `modelBuilder.ApplyConfigurationsFromAssembly()`
- ✅ Domain entities have navigation properties, no circular dependencies

### 2. Service Pattern
- Interface: `IAuthService`, `ITeamService`, etc. (in `Application/Services/Interfaces/`)
- Implementation: `AuthService`, `TeamService`, etc. (in `Application/Services/Implementations/`)
- ✅ Always inject `AppDbContext` via constructor
- ✅ Use `async/await` for all DB operations
- ✅ Return `null` for not-found scenarios (not exceptions)

### 3. Dependency Injection (DI)
```csharp
// In Program.cs (Klava.UI)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<SessionService>(); // UI session state
```
- ✅ Use `Scoped` lifetime for all services (Blazor Server circuit-scoped)

### 4. Blazor Component Standards
- ✅ Add `@rendermode InteractiveServer` to pages with forms/interactivity
- ✅ Inject services via `@inject IServiceName ServiceName`
- ✅ Use `<EditForm>` with `Model="@model"` and `OnValidSubmit`
- ✅ Use `[Required]`, `[MaxLength]` in component models (not Domain entities)
- ✅ Implement `OnInitializedAsync()` for data loading
- ✅ Subscribe to `SessionService.OnUserChanged` for auth state updates

### 5. DTOs for Data Transfer
- Create DTOs in `Application/DTOs/` (e.g., `UserDto`, `TeamDto`)
- ✅ Use DTOs between UI and Application layers
- ✅ Map Domain entities to DTOs in services (not in UI)

### 6. Navigation & Sessions
```csharp
// SessionService (Klava.UI/Services/)
public User? CurrentUser { get; private set; }
public event Action? OnUserChanged;
public bool IsAuthenticated => CurrentUser != null;
```
- ✅ Use `NavigationManager.NavigateTo()` for redirects
- ✅ Check `Session.IsAuthenticated` before protected operations

---

## 🔒 Security & Business Logic

### Password Hashing (BCrypt)
```csharp
// Hash password
var hashed = BCrypt.Net.BCrypt.HashPassword(plainPassword);

// Verify password
bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
```

### Team Code Generation
- 8 characters: `A-Z`, `0-9` only
- Must be unique (check `_context.Teams.AnyAsync(t => t.Code == code)`)
- Store uppercase in DB

### Authorization Rules
- **Headman**: Can create/edit/delete tasks, manage members, promote/demote roles
- **Student**: Read-only access to tasks, cannot manage members
- Check role: `IMemberService.IsHeadmanAsync(userId, teamId)`

---

## 📁 File Locations (Quick Reference)

```
src/
├─ Klava.Domain/
│  ├─ Entities/       → User.cs, Team.cs, TeamMember.cs, Subject.cs, Task.cs
│  └─ Enums/          → TeamMemberRole.cs, SubjectStatus.cs
│
├─ Klava.Infrastructure/
│  └─ Data/
│     ├─ AppDbContext.cs
│     └─ Configurations/  → UserConfiguration.cs, TeamConfiguration.cs, etc.
│
├─ Klava.Application/
│  ├─ Services/
│  │  ├─ Interfaces/      → IAuthService.cs, ITeamService.cs, etc.
│  │  └─ Implementations/ → AuthService.cs, TeamService.cs, etc.
│  └─ DTOs/               → UserDto.cs, TeamDto.cs, TaskDto.cs, etc.
│
└─ Klava.UI/
   ├─ Components/
   │  ├─ Pages/
   │  │  ├─ Authentication/ → Login.razor, Register.razor
   │  │  ├─ Teams/          → TeamList.razor, CreateTeam.razor, TeamDashboard.razor
   │  │  └─ Tasks/          → TaskList.razor
   │  └─ Layout/            → MainLayout.razor, NavMenu.razor
   ├─ Services/             → SessionService.cs
   ├─ Program.cs            → DI registration, DbContext config
   └─ appsettings.json      → Connection string
```

---

## 🧪 EF Core Migrations

```bash
# Create migration (from Infrastructure project)
cd src/Klava.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../Klava.UI

# Apply migration
dotnet ef database update --startup-project ../Klava.UI
```
⚠️ Always specify `--startup-project` (connection string is in UI project)

---

## 🔍 Common Patterns

### Service Method Example
```csharp
public async Task<Team?> GetTeamByCodeAsync(string code)
{
    return await _context.Teams
        .Include(t => t.Members)
        .ThenInclude(m => m.User)
        .FirstOrDefaultAsync(t => t.Code == code);
}
```
- ✅ Use `.Include()` for navigation properties
- ✅ Return `null` if not found (don't throw)

### Blazor Page Pattern
```razor
@page "/path"
@rendermode InteractiveServer
@inject IService Service
@inject SessionService Session

<PageTitle>Title</PageTitle>

@if (!Session.IsAuthenticated) { return; }

@if (data == null)
{
    <p>Loading...</p>
}
else
{
    <!-- Render data -->
}

@code {
    private List<Model>? data;

    protected override async Task OnInitializedAsync()
    {
        data = await Service.GetDataAsync();
    }
}
```

### Entity Configuration Example
```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Firstname)
            .HasColumnName("firstname")
            .HasMaxLength(100)
            .IsRequired();
    }
}
```
- ✅ Use `snake_case` for table/column names (PostgreSQL convention)
- ✅ Configure constraints, indexes, relationships here (not in Domain)

---

## ✅ Current Implementation Status

- ✅ **Phase 1**: Domain entities, EF configurations, DbContext, migrations
- 🔄 **Phase 2**: Services (IAuthService, ITeamService, IMemberService, ITaskService, ISubjectService)
- ⏳ **Phase 3**: Blazor UI components (Login, Register, TeamList, TaskList, etc.)

---

## 📌 Key Reminders for AI

1. **NEVER add data annotations to Domain entities** → Use Fluent API in Infrastructure
2. **Always use `Scoped` lifetime** for services in Blazor Server
3. **Include `@rendermode InteractiveServer`** on pages with forms/events
4. **Check `Session.IsAuthenticated`** before accessing user data
5. **Use DTOs** for data transfer between Application and UI layers
6. **Follow naming**: `IService` → `Service` (interface → implementation)
7. **PostgreSQL enums**: Store as lowercase strings, convert via `.HasConversion()`
8. **Team codes**: Always uppercase, 8 chars, alphanumeric only
9. **Passwords**: Hash with BCrypt, never store plain text
10. **Cascade deletes**: Team → Subject → Task (configured in Fluent API)

---

**End of Context** — This file provides all architectural, technical, and structural context needed to assist with development tasks.
