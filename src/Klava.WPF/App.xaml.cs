using System.IO;
using System.Windows;
using WpfApp = System.Windows.Application;
using Klava.Application.Services.Implementations;
using Klava.Application.Services.Interfaces;
using Klava.Infrastructure.Data;
using Klava.WPF.Services;
using Klava.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

                // Register DbContext
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString), ServiceLifetime.Transient);

                // Register Application Services
                services.AddTransient<IAuthService, AuthService>();
                services.AddTransient<ITeamService, TeamService>();
                services.AddTransient<IMemberService, MemberService>();
                services.AddTransient<ITaskService, TaskService>();
                services.AddTransient<ISubjectService, SubjectService>();

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

