using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using RestSharp;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// =====================
// Logging con Serilog
// =====================
var webLogPath = Path.Combine(AppContext.BaseDirectory, "Logs", "web-.log");
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.File(webLogPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

// =====================
// MVC + Vistas
// =====================
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

// =====================
// Autenticación (Cookies)
// =====================
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Ingresar/Login";          // página de login
        options.LogoutPath = "/Ingresar/Logout";        // acción de logout
        options.AccessDeniedPath = "/Ingresar/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        // Opcional: nombre de la cookie
        options.Cookie.Name = "WebAuthCookie";
    });

builder.Services.AddAuthorization(options =>
{
    // Política por defecto: requiere usuario autenticado
    options.FallbackPolicy = options.DefaultPolicy;
});

// =====================
// Cliente para consumir el API
// =====================

// BaseAddress del API (ajústalo según tu entorno)
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl");
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException(
        "La configuración 'ApiBaseUrl' no está definida para el proyecto Web.");
}

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});



var app = builder.Build();

// =====================
// Pipeline HTTP
// =====================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// =====================
// Rutas MVC
// =====================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();