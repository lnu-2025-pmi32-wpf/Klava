using Klava.UI.Components;
using Klava.UI.Services;
using Klava.Infrastructure.Data;
using Klava.Infrastructure.Interfaces;
using Klava.Infrastructure.Services;
using Klava.Application.Services.Interfaces;
using Klava.Application.Services.Implementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.MapEnum<Klava.Domain.Enums.TeamMemberRole>("team_member_role")
              .MapEnum<Klava.Domain.Enums.SubjectStatus>("subject_status")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISubjectFileService, SubjectFileService>();

var fileStoragePath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
builder.Services.AddScoped<IFileStorageService>(sp => new FileStorageService(fileStoragePath));

builder.Services.AddScoped<SessionService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/api/files/download/{fileId:int}", async (int fileId, ISubjectFileService fileService) =>
{
    var file = await fileService.GetFileByIdAsync(fileId);
    if (file == null)
        return Results.NotFound();

    var stream = await fileService.DownloadFileAsync(fileId);
    if (stream == null)
        return Results.NotFound();

    return Results.File(stream, file.ContentType, file.DisplayName);
});

app.Run();
