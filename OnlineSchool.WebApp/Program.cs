using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OnlineSchool.WebApp.Components;
using OnlineSchool.WebApp.Services;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ����������� �������� ---

// ��������� ������� ������� Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ��������� ������� ��� �������������� �� ������� �������
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();


builder.Services.AddAuthentication("CustomScheme")
    .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("CustomScheme", null);

// ��������� ������ ��� ������ � Local Storage
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<TokenStorageService>();

// ����������� HttpClient ��� ������� � ����� API
builder.Services.AddHttpClient("OnlineSchool.API", client =>
{
    // ���������, ��� ����� ����������!
    client.BaseAddress = new Uri("https://localhost:7078");
});

// ������������ ��� ���� ��������� ������� ��� ������ � API
// ��� ����� ������������ HttpClient, ����������� ����
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProgramService, ProgramService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();


var app = builder.Build();


// --- 2. ��������� ��������� �������� ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// ����������� �������� ����� Blazor
// ��� ������������ � ���������� ����� MapRazorComponents
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();