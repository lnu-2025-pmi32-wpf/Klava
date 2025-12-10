using System.IO;
using System.Windows;
using System.Windows.Threading;
using WpfApp = System.Windows.Application;
using Klava.Application.Services.Implementations;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;
using Klava.Infrastructure.Logging;
using Klava.WPF.Services;
using Klava.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Serilog;

namespace Klava.WPF;

public partial class App : WpfApp
{
    private readonly IHost _host;

    public App()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureKlavaLogging()
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString, npgsqlOptions =>
                    {
                        npgsqlOptions.MapEnum<TeamMemberRole>("team_member_role");
                        npgsqlOptions.MapEnum<SubjectStatus>("subject_status");
                        npgsqlOptions.MapEnum<SubmissionStatus>("submission_status");
                    }), ServiceLifetime.Transient);

                services.AddTransient<IAuthService, AuthService>();
                services.AddTransient<ITeamService, TeamService>();
                services.AddTransient<IMemberService, MemberService>();
                services.AddTransient<ITaskService, TaskService>();
                services.AddTransient<ISubjectService, SubjectService>();
                services.AddTransient<ISubjectFileService, SubjectFileService>();
                services.AddTransient<ISubmissionService, SubmissionService>();

                var fileStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                services.AddSingleton<Klava.Infrastructure.Interfaces.IFileStorageService>(
                    sp => new Klava.Infrastructure.Services.FileStorageService(fileStoragePath));

                services.AddSingleton<SessionService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IDialogService, DialogService>();

                services.AddTransient<MainViewModel>();
                services.AddTransient<LoginViewModel>();
                services.AddTransient<RegisterViewModel>();
                services.AddTransient<TeamListViewModel>();
                services.AddTransient<CreateTeamViewModel>();
                services.AddTransient<TeamDashboardViewModel>();
                services.AddTransient<ManageMembersViewModel>();
                services.AddTransient<SubjectListViewModel>();
                services.AddTransient<SubjectDetailsViewModel>();
                services.AddTransient<TaskListViewModel>();

                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Klava application starting...");

        await _host.StartAsync();

        var navigationService = _host.Services.GetRequiredService<INavigationService>();
        navigationService.NavigateTo<LoginViewModel>();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        logger.LogInformation("Klava application started successfully");

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Klava application shutting down...");

        await _host.StopAsync();
        _host.Dispose();

        Log.CloseAndFlush();

        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogError(e.Exception, "Unhandled UI exception");
        e.Handled = true;
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
}

