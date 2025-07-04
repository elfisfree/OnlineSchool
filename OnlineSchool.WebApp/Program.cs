using OnlineSchool.WebApp.Components;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OnlineSchool.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddBlazoredLocalStorage();

// 2. Настраиваем HttpClient для общения с нашим API
builder.Services.AddHttpClient("OnlineSchool.API", client =>
{
    // Укажите адрес, по которому запускается ваш API.
    // Этот адрес можно найти в файле launchSettings.json проекта API
    client.BaseAddress = new Uri("https://localhost:7078"); // ЗАМЕНИТЕ НА СВОЙ АДРЕС API!
});

builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProgramService, ProgramService>();



builder.Services.AddAuthorizationCore();

var app = builder.Build();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
