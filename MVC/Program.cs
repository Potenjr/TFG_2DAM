using IronGym.Shared.Services;
using IronGym.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrar la licencia de Syncfusion (si aplica)
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBPh8sVXJyS0d+X1RPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9nSXhRfkRnWHxceHxdT2k=");

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Configuraci�n de autenticaci�n mediante cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token"; // Nombre de la cookie
        options.LoginPath = "/employee/login"; // Ruta de inicio de sesi�n
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30); // Tiempo de expiraci�n
        options.AccessDeniedPath = "/access-denied"; // Ruta si el acceso es denegado
    });

builder.Services.AddAuthorization();

// Registro de servicios personalizados
builder.Services.AddScoped<IAESService, AESService>();
builder.Services.AddHttpClient<IRequestSenderService, RequestSenderService>();

var app = builder.Build();

// Configuraci�n del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    // P�gina de error en caso de excepci�n
    app.UseExceptionHandler("/Home/Error");

    // Forzar uso de HTTPS con pol�tica HSTS (HTTP Strict Transport Security)
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirigir todas las solicitudes HTTP a HTTPS
app.UseStaticFiles(); // Servir archivos est�ticos (CSS, JS, im�genes, etc.)

app.UseRouting(); // Habilitar el enrutamiento

app.UseAuthentication(); // Activar autenticaci�n
app.UseAuthorization();  // Activar autorizaci�n

// Definici�n de la ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); // Ejecutar la aplicaci�n
