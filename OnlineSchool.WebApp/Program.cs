using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OnlineSchool.WebApp.Components;
using OnlineSchool.WebApp.Services;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// --- 1. РЕГИСТРАЦИЯ СЕРВИСОВ ---

// Добавляем базовые сервисы Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Добавляем сервисы для аутентификации на стороне клиента
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();


builder.Services.AddAuthentication("CustomScheme")
    .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("CustomScheme", null);

// Добавляем сервис для работы с Local Storage
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<TokenStorageService>();

// Настраиваем HttpClient для общения с нашим API
builder.Services.AddHttpClient("OnlineSchool.API", client =>
{
    // УБЕДИТЕСЬ, ЧТО АДРЕС ПРАВИЛЬНЫЙ!
    client.BaseAddress = new Uri("https://localhost:7078");
});

// Регистрируем все наши кастомные сервисы для работы с API
// Они будут использовать HttpClient, настроенный выше
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProgramService, ProgramService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();


var app = builder.Build();


// --- 2. НАСТРОЙКА КОНВЕЙЕРА ЗАПРОСОВ ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Настраиваем конечные точки Blazor
// Это единственный и правильный вызов MapRazorComponents
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();