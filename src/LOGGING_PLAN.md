# LOGGING_PLAN.md

## Logging Implementation Plan for Klava

This document outlines the implementation plan for a file-system logging mechanism in the Klava WPF application.

---

## 1. Library Selection

### Recommended: **Serilog**

| Criteria | Serilog | NLog | Microsoft.Extensions.Logging (built-in) |
|----------|---------|------|----------------------------------------|
| .NET 9 Support | ✅ Excellent | ✅ Good | ✅ Native |
| File Sink | ✅ Built-in | ✅ Built-in | ❌ Requires 3rd party |
| Log Rotation | ✅ Native (rolling files) | ✅ Native | ❌ Manual |
| Structured Logging | ✅ Excellent | ✅ Good | ⚠️ Basic |
| DI Integration | ✅ `Microsoft.Extensions.Logging` compatible | ✅ Compatible | ✅ Native |
| Configuration | ✅ JSON/Code | ✅ XML/JSON | ✅ JSON |
| Performance | ✅ High (async sinks) | ✅ High | ✅ High |

### Why Serilog?
- **Seamless integration** with `Microsoft.Extensions.Hosting` (already used in `App.xaml.cs`)
- **Rolling file support** out of the box (daily, size-based)
- **Separate sinks** for different log levels (errors to separate file)
- **Structured logging** with property enrichment
- **Minimal configuration** via `appsettings.json`

### Required NuGet Packages

```xml
<!-- Klava.WPF.csproj -->
<PackageReference Include="Serilog" Version="4.1.0" />
<PackageReference Include="Serilog.Extensions.Hosting" Version="8.0.0" />
<PackageReference Include="Serilog.Settings.Configuration" Version="8.0.4" />
<PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
<PackageReference Include="Serilog.Enrichers.Thread" Version="4.0.0" />
<PackageReference Include="Serilog.Enrichers.Environment" Version="3.0.1" />
```

---

## 2. Structure & Placement

### File Organization

```
Klava/
├── src/
│   ├── Klava.Infrastructure/
│   │   ├── Logging/                     # NEW: Logging infrastructure
│   │   │   └── LoggingConfiguration.cs  # Extension methods for Serilog setup
│   │   └── Interfaces/
│   │       └── IFileStorageService.cs
│   │
│   ├── Klava.WPF/
│   │   ├── App.xaml.cs                  # MODIFY: Add Serilog to Host
│   │   ├── appsettings.json             # MODIFY: Add Serilog configuration
│   │   └── logs/                        # OUTPUT: Log files directory
│   │       ├── klava-.log               # All logs (rolling daily)
│   │       └── klava-errors-.log        # Errors only (rolling daily)
```

### Architecture Integration

Following the project's Clean Architecture:

```
┌─────────────────────────────────────────────────────────────┐
│                    Klava.WPF (Presentation)                 │
│   App.xaml.cs configures Serilog via Host.UseSerilog()      │
│   ViewModels inject ILogger<T> for UI-level logging         │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│                  Klava.Application (Business)               │
│   Services inject ILogger<T> for business logic logging     │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│                Klava.Infrastructure (Data Access)           │
│   LoggingConfiguration.cs provides setup extensions         │
│   DbContext logs via EF Core integration                    │
└─────────────────────────────────────────────────────────────┘
```

---

## 3. File Strategy

### Log File Organization

| File Pattern | Content | Rotation | Retention |
|--------------|---------|----------|-----------|
| `logs/klava-{Date}.log` | All logs (Info, Warning, Error) | Daily | 30 days |
| `logs/klava-errors-{Date}.log` | Errors only | Daily | 90 days |

### Rolling Policy

```
logs/
├── klava-20251210.log        # Today's general log
├── klava-20251209.log        # Yesterday
├── klava-20251208.log        # ...
├── klava-errors-20251210.log # Today's errors
├── klava-errors-20251209.log # Yesterday's errors
└── ...
```

### Log Format

```
[2025-12-10 14:30:45.123 +02:00] [INF] [AuthService] User "John Doe" logged in successfully
[2025-12-10 14:31:02.456 +02:00] [WRN] [TeamService] Team code "ABC123" not found
[2025-12-10 14:32:15.789 +02:00] [ERR] [TaskService] Failed to create task: Database connection timeout
    Exception: Npgsql.NpgsqlException: Connection timeout
       at Npgsql.NpgsqlConnection.Open()
       ...
```

### Log Levels Mapping

| Level | Usage | Example |
|-------|-------|---------|
| **Information** | Normal operations, user actions | User login, team created, file uploaded |
| **Warning** | Potential issues, recoverable errors | Invalid input, missing optional data |
| **Error** | Failures requiring attention | Database errors, file I/O failures, exceptions |

---

## 4. Implementation Steps

### Checklist

- [ ] **Step 1**: Install NuGet packages in `Klava.WPF`
- [ ] **Step 2**: Create `LoggingConfiguration.cs` in `Klava.Infrastructure/Logging/`
- [ ] **Step 3**: Update `appsettings.json` with Serilog configuration
- [ ] **Step 4**: Modify `App.xaml.cs` to use Serilog
- [ ] **Step 5**: Add `ILogger<T>` to Application Services
- [ ] **Step 6**: Add `ILogger<T>` to ViewModels
- [ ] **Step 7**: Add global exception handling
- [ ] **Step 8**: Test logging output
- [ ] **Step 9**: Add `.gitignore` entry for logs folder

---

## 5. Code Examples

### Step 1: Install Packages

```powershell
cd src\Klava.WPF
dotnet add package Serilog --version 4.1.0
dotnet add package Serilog.Extensions.Hosting --version 8.0.0
dotnet add package Serilog.Settings.Configuration --version 8.0.4
dotnet add package Serilog.Sinks.File --version 6.0.0
dotnet add package Serilog.Sinks.Console --version 6.0.0
dotnet add package Serilog.Enrichers.Thread --version 4.0.0
dotnet add package Serilog.Enrichers.Environment --version 3.0.1
```

### Step 2: LoggingConfiguration.cs

**File**: `Klava.Infrastructure/Logging/LoggingConfiguration.cs`

```csharp
namespace Klava.Infrastructure.Logging;

using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

public static class LoggingConfiguration
{
    public static IHostBuilder ConfigureKlavaLogging(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) =>
        {
            var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            Directory.CreateDirectory(logsPath);

            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProperty("Application", "Klava")
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine(logsPath, "klava-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine(logsPath, "klava-errors-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 90,
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");
        });
    }
}
```

### Step 3: Update appsettings.json

**File**: `Klava.WPF/appsettings.json`

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=klava_db;Username=postgres;Password=admin"
  }
}
```

### Step 4: Modify App.xaml.cs

**File**: `Klava.WPF/App.xaml.cs`

```csharp
using System.IO;
using System.Windows;
using WpfApp = System.Windows.Application;
using Klava.Application.Services.Implementations;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;
using Klava.Infrastructure.Logging;  // NEW
using Klava.WPF.Services;
using Klava.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;  // NEW
using Serilog;  // NEW

namespace Klava.WPF;

public partial class App : WpfApp
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureKlavaLogging()  // NEW: Add Serilog
            .ConfigureServices((context, services) =>
            {
                // ... existing service registrations ...
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        // NEW: Log application startup
        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Klava application starting...");

        await _host.StartAsync();

        var navigationService = _host.Services.GetRequiredService<INavigationService>();
        navigationService.NavigateTo<LoginViewModel>();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        logger.LogInformation("Klava application started successfully");  // NEW

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        // NEW: Log application shutdown
        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Klava application shutting down...");

        await _host.StopAsync();
        _host.Dispose();

        Log.CloseAndFlush();  // NEW: Ensure all logs are written

        base.OnExit(e);
    }
}
```

### Step 5: Add Logging to Application Services

**Example**: `Klava.Application/Services/Implementations/AuthService.cs`

```csharp
namespace Klava.Application.Services.Implementations;

using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;  // NEW
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Infrastructure.Data;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuthService> _logger;  // NEW

    public AuthService(AppDbContext context, ILogger<AuthService> logger)  // MODIFIED
    {
        _context = context;
        _logger = logger;  // NEW
    }

    public async Task<User?> RegisterAsync(string firstname, string lastname, string password)
    {
        _logger.LogInformation("Attempting to register user: {Firstname} {Lastname}", firstname, lastname);

        if (await UserExistsAsync(firstname, lastname))
        {
            _logger.LogWarning("Registration failed: User {Firstname} {Lastname} already exists", firstname, lastname);
            return null;
        }

        try
        {
            var hashedPassword = BCrypt.HashPassword(password);
            var user = new User
            {
                Firstname = firstname,
                Lastname = lastname,
                Password = hashedPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User registered successfully: {Firstname} {Lastname} (ID: {UserId})", 
                firstname, lastname, user.Id);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register user: {Firstname} {Lastname}", firstname, lastname);
            throw;
        }
    }

    public async Task<User?> LoginAsync(string firstname, string lastname, string password)
    {
        _logger.LogInformation("Login attempt for user: {Firstname} {Lastname}", firstname, lastname);

        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Firstname == firstname && u.Lastname == lastname);

            if (user == null)
            {
                _logger.LogWarning("Login failed: User {Firstname} {Lastname} not found", firstname, lastname);
                return null;
            }

            if (!BCrypt.Verify(password, user.Password))
            {
                _logger.LogWarning("Login failed: Invalid password for user {Firstname} {Lastname}", firstname, lastname);
                return null;
            }

            _logger.LogInformation("User logged in successfully: {Firstname} {Lastname} (ID: {UserId})", 
                firstname, lastname, user.Id);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for user: {Firstname} {Lastname}", firstname, lastname);
            throw;
        }
    }

    public async Task<bool> UserExistsAsync(string firstname, string lastname)
    {
        return await _context.Users
            .AnyAsync(u => u.Firstname == firstname && u.Lastname == lastname);
    }
}
```

### Step 6: Add Logging to ViewModels

**Example**: `Klava.WPF/ViewModels/LoginViewModel.cs`

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;  // NEW
using Klava.Application.Services.Interfaces;
using Klava.WPF.Services;

namespace Klava.WPF.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly SessionService _sessionService;
    private readonly ILogger<LoginViewModel> _logger;  // NEW

    [ObservableProperty]
    private string _firstname = string.Empty;

    [ObservableProperty]
    private string _lastname = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public LoginViewModel(
        IAuthService authService,
        INavigationService navigationService,
        SessionService sessionService,
        ILogger<LoginViewModel> logger)  // MODIFIED
    {
        _authService = authService;
        _navigationService = navigationService;
        _sessionService = sessionService;
        _logger = logger;  // NEW
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        _logger.LogInformation("Login command executed");

        try
        {
            var user = await _authService.LoginAsync(Firstname, Lastname, Password);

            if (user != null)
            {
                _sessionService.SetCurrentUser(user);
                _logger.LogInformation("Navigation to TeamListView after successful login");
                _navigationService.NavigateTo<TeamListViewModel>();
            }
            else
            {
                ErrorMessage = "Невірні дані для входу";
                _logger.LogWarning("Login failed - invalid credentials displayed to user");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Помилка підключення до бази даних";
            _logger.LogError(ex, "Database connection error during login");
        }
    }

    [RelayCommand]
    private void NavigateToRegister()
    {
        _logger.LogInformation("Navigating to RegisterView");
        _navigationService.NavigateTo<RegisterViewModel>();
    }
}
```

### Step 7: Global Exception Handling

**Add to**: `Klava.WPF/App.xaml.cs`

```csharp
public App()
{
    // Add global exception handlers
    DispatcherUnhandledException += OnDispatcherUnhandledException;
    AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
    TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

    _host = Host.CreateDefaultBuilder()
        // ... existing configuration ...
        .Build();
}

private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
{
    var logger = _host.Services.GetRequiredService<ILogger<App>>();
    logger.LogError(e.Exception, "Unhandled UI exception");
    e.Handled = true; // Prevent app crash, show error dialog instead
    MessageBox.Show($"Сталася помилка: {e.Exception.Message}", "Помилка", 
        MessageBoxButton.OK, MessageBoxImage.Error);
}

private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    var logger = _host.Services.GetRequiredService<ILogger<App>>();
    logger.LogError((Exception)e.ExceptionObject, "Unhandled domain exception");
}

private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
{
    var logger = _host.Services.GetRequiredService<ILogger<App>>();
    logger.LogError(e.Exception, "Unobserved task exception");
    e.SetObserved();
}
```

### Step 9: Update .gitignore

**Add to**: `.gitignore`

```gitignore
# Logs
**/logs/
*.log
```

---

## 6. Testing the Implementation

### Manual Testing Checklist

1. **Start the application** → Check `logs/klava-{date}.log` is created
2. **Login with valid credentials** → Verify "User logged in successfully" in logs
3. **Login with invalid credentials** → Verify warning in logs
4. **Create a team** → Verify info log entry
5. **Trigger an error** (e.g., disconnect database) → Verify error in `logs/klava-errors-{date}.log`
6. **Close the application** → Verify "application shutting down" log entry

### Sample Log Output

```
[2025-12-10 14:30:45.123 +02:00] [INF] [Klava.WPF.App] Klava application starting...
[2025-12-10 14:30:46.456 +02:00] [INF] [Klava.WPF.App] Klava application started successfully
[2025-12-10 14:31:02.789 +02:00] [INF] [LoginViewModel] Login command executed
[2025-12-10 14:31:03.012 +02:00] [INF] [AuthService] Login attempt for user: John Doe
[2025-12-10 14:31:03.234 +02:00] [INF] [AuthService] User logged in successfully: John Doe (ID: 1)
[2025-12-10 14:31:03.345 +02:00] [INF] [LoginViewModel] Navigation to TeamListView after successful login
```

---

## 7. Quick Reference

### Log Level Usage

```csharp
// Information - Normal operations
_logger.LogInformation("User {UserId} created team {TeamName}", userId, teamName);

// Warning - Potential issues
_logger.LogWarning("Team code {Code} not found", code);

// Error - Failures with exception
_logger.LogError(ex, "Failed to save submission for task {TaskId}", taskId);
```

### Structured Logging Best Practices

```csharp
// DO: Use structured logging with named placeholders
_logger.LogInformation("User {FirstName} {LastName} joined team {TeamId}", user.Firstname, user.Lastname, teamId);

// DON'T: Use string interpolation
_logger.LogInformation($"User {user.Firstname} {user.Lastname} joined team {teamId}"); // Avoid!
```

---

## 8. Future Enhancements

- [ ] Add Serilog Seq sink for centralized log viewing
- [ ] Add performance logging middleware
- [ ] Implement log correlation IDs for request tracing
- [ ] Add log analytics dashboard
- [ ] Configure log level via environment variables for production
