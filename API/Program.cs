using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using IronGym.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor

builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI para la documentación de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Cabecera estándar de autorización utilizando el esquema Bearer. Ejemplo: \"Bearer {token}\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Registrar el contexto de base de datos
builder.Services.AddDbContext<IronGymContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IronGym"));
});

// Registrar servicios personalizados
builder.Services.AddScoped<ISecurityService>(serviceProvider =>
{
    var appSettingsToken = builder.Configuration.GetSection("AppSettings:Token").Value;
    return new SecurityService(appSettingsToken);
});
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AESService>();

// Configurar autenticación y autorización mediante JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Asegurar que la base de datos esté creada y migraciones aplicadas al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<IronGymContext>();

        // Crear la base de datos si no existe
        context.Database.EnsureCreated();

        // Aplicar migraciones pendientes
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Manejar errores durante la migración o inicialización
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ha ocurrido un error al migrar o inicializar la base de datos.");
    }
}

app.Run();
