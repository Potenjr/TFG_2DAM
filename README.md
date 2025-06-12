# Potengym 🏋️‍♀️
**Potengym** es una aplicación web desarrollada como parte del Trabajo de Fin de Ciclo del Grado Superior en Desarrollo de Aplicaciones Multiplataforma.  
Está orientada a la gestión de gimnasios, ofreciendo funcionalidades como autenticación de usuarios, control de pagos, gestión de rutinas y administración de clientes.

## 🧱 Arquitectura del proyecto
El proyecto sigue una arquitectura en capas y está dividido en dos ejecutables principales:

IronGym/
├── API/               # Proyecto ASP.NET Core API  
├── MVC/               # Proyecto ASP.NET Core MVC (frontend)  
├── Application/       # Lógica de aplicación  
├── Domain/            # Entidades 
├── Infrastructure/    # Acceso a datos y configuración  
├── IronGym.Shared/    # Servicios compartidos (e.g. encriptación)  
├── AESServiceTests/   # Tests de servicios  
├── APITests/          # Tests de la API  
├── Application.Tests/ # Tests de la capa de aplicación  
├── Infrastructure.Tests/  
├── MVC.Tests/  
├── IronGym.sln        # Solución principal  

## 🚀 Ejecución local

### Requisitos
- [.NET SDK 8.0](https://dotnet.microsoft.com/download)
- Visual Studio 2022+ o terminal con CLI de .NET
- SQL Server (localdb o instancia)

### Paso 1: Ejecutar la API
1. Abre una terminal en la carpeta `API`
2. Ejecuta:
   dotnet run
3. La API estará disponible en https://localhost:7175
4. Puedes acceder a Swagger para ver los endpoints en https://localhost:7175/swagger

### Paso 2: Ejecutar el proyecto MVC
1. Abre una segunda terminal en la carpeta `MVC`
2. Ejecuta:
   dotnet run
3. Se abrirá automáticamente en el navegador o estará disponible en https://localhost:xxxx

🔁 Asegúrate de que el proyecto MVC tenga configurada la URL de la API correctamente.  
Por ejemplo, en `appsettings.json` o donde inicialices `HttpClient`:

"ApiBaseUrl": "https://localhost:7175"

## 🔐 Autenticación
- El sistema usa tokens JWT.
- Al iniciar sesión, la API devuelve un token que debe enviarse en la cabecera `Authorization` como:
  Authorization: Bearer {token}
- Swagger permite probar los endpoints protegidos usando este token.

## 🧪 Pruebas
El proyecto incluye pruebas unitarias para:
- API
- Application
- Infrastructure
- MVC
- Servicios compartidos

Para ejecutarlas todas:
dotnet test

## 📦 Paquetes NuGet utilizados
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.AspNetCore.Authentication.JwtBearer
- Swashbuckle.AspNetCore (Swagger)
- xUnit y Moq (para testing)
- Coverlet (para cobertura de código)

## 👨‍💻 Autor
Desarrollado por **Adrián Potenciano Vila**  
IES Luis Vives — 2º Desarrollo de Aplicaciones Multiplataforma
Curso 2024/25

## 📝 Licencia
Este proyecto es de uso académico. Puede utilizarse libremente con fines educativos o de demostración.
