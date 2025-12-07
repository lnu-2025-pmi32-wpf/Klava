using System.IO;
using System.Windows;
using WpfApp = System.Windows.Application;
using Klava.Application.Services.Implementations;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;
using Klava.WPF.Services;
using Klava.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Klava.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
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
            .ConfigureServices((context, services) =>
            {
                // Configuration
                var configuration = context.Configuration;
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                // Enable legacy timestamp behavior globally for Npgsql
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                // Register DbContext with enum mapping configuration
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString, npgsqlOptions =>
                    {
                        npgsqlOptions.MapEnum<TeamMemberRole>("team_member_role");
                        npgsqlOptions.MapEnum<SubjectStatus>("subject_status");
                        npgsqlOptions.MapEnum<SubmissionStatus>("submission_status");
                    }), ServiceLifetime.Transient);

                // Register Application Services
                services.AddTransient<IAuthService, AuthService>();
                services.AddTransient<ITeamService, TeamService>();
                services.AddTransient<IMemberService, MemberService>();
                services.AddTransient<ITaskService, TaskService>();
                services.AddTransient<ISubjectService, SubjectService>();
                services.AddTransient<ISubjectFileService, SubjectFileService>();
                services.AddTransient<ISubmissionService, SubmissionService>();

                // Configure file storage
                var fileStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                services.AddSingleton<Klava.Infrastructure.Interfaces.IFileStorageService>(
                    sp => new Klava.Infrastructure.Services.FileStorageService(fileStoragePath));

                // Register UI Services
                services.AddSingleton<SessionService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IDialogService, DialogService>();

                // Register ViewModels
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

                // Register MainWindow
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        var navigationService = _host.Services.GetRequiredService<INavigationService>();
        navigationService.NavigateTo<LoginViewModel>();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}

