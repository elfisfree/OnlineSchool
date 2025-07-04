// --- НАЧАЛО ФАЙЛА OnlineSchool.API/Program.cs ---

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineSchool.API.Data;
using OnlineSchool.Shared.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. РЕГИСТРАЦИЯ СЕРВИСОВ

// Подключение к базе данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Настройка ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Настройки пароля (опционально, но рекомендуется)
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Настройка Аутентификации с использованием JWT
// ЭТО САМЫЙ ВАЖНЫЙ БЛОК ДЛЯ ИСПРАВЛЕНИЯ ОШИБКИ
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Для разработки ставим false
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// 2. НАСТРОЙКА КОНВЕЙЕРА ОБРАБОТКИ ЗАПРОСОВ (MIDDLEWARE)

// Инициализация ролей при запуске
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        // Убедимся, что наш сервис для инициализации есть или создадим его здесь же
        string[] roleNames = { "Admin", "Teacher", "Student" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database roles.");
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Важно: UseRouting должен быть до UseAuthentication и UseAuthorization
app.UseRouting();

// Включаем аутентификацию и авторизацию. ПОРЯДОК КРИТИЧЕН!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// --- КОНЕЦ ФАЙЛА ---