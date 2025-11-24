using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Microsoft.OpenApi.Models;
using Aplicacion.Servicios;
using Aplicacion.Interfaces;
using Dominio.DetalleVentas;
using Dominio.EncabezadoVentas;
using Dominio.Productos;
using Infraestructura.Data;
using Infraestructura.Data.DetalleVentas;
using Infraestructura.Data.EncabezadoVentas;
using Infraestructura.Data.Productos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for API project
var apiLogPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Logs", "api-.log");
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.File(apiLogPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

// Add services to the container.

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Servicios + Repositorios
var connStr = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connStr));

builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductosService, ProductosService>();

builder.Services.AddScoped<IEncabezadoVentaRepository, EncabezadoVentaRepository>();
builder.Services.AddScoped<IEncabezadoVentasService, EncabezadoVentasService>();

builder.Services.AddScoped<IDetalleVentaRepository, DetalleVentaRepository>();
builder.Services.AddScoped<IDetalleVentasService, DetalleVentasService>();

//// File storage
// var uploadsPath = Path.Combine(builder.Environment.WebRootPath, "uploads", "productos");
// builder.Services.AddSingleton<IFileStorageService>(sp => new FileStorageService(uploadsPath));

// Autenticación JWT (desde configuración)
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection.GetValue<string>("Issuer") ?? string.Empty;
var audience = jwtSection.GetValue<string>("Audience") ?? string.Empty;
var secret = jwtSection.GetValue<string>("Secret") ?? string.Empty;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Política por rol/claim
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin")); // o .RequireClaim("permission", "x")
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configuración de seguridad JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Autenticación JWT. En Authorize ingrese SOLO el token (sin 'Bearer')."
    });

    // Requerimiento de seguridad global
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();