using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TrabajoProyecto.Services;

// Crear la aplicación web usando el patrón Builder
var builder = WebApplication.CreateBuilder(args);

// Configurar los servicios del contenedor de dependencias
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar autenticación JWT (JSON Web Tokens)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validar que el emisor del token es confiable
            ValidateIssuer = true,
            // Validar que el destinatario del token es correcto
            ValidateAudience = true,
            // Validar que el token no ha expirado
            ValidateLifetime = true,
            // Validar la firma del token para asegurar su autenticidad
            ValidateIssuerSigningKey = true,
            // Obtener el emisor válido desde la configuración
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            // Obtener el destinatario válido desde la configuración
            ValidAudience = builder.Configuration["Jwt:Audience"],
            // Clave secreta para verificar la firma del token
            // Usa una clave por defecto si no está configurada (solo para desarrollo)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "SuperSecretKeyForDevelopmentOnly123!"))
        };
    });

// Habilitar autorización para controlar acceso a recursos
builder.Services.AddAuthorization();

// Registrar servicios personalizados en el contenedor de dependencias
// AddScoped crea una nueva instancia por cada solicitud HTTP
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Construir la aplicación
var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    // Habilitar Swagger/OpenAPI solo en entorno de desarrollo
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Especificar el endpoint de la documentación Swagger
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "🏆 API Gestión de Clubes v1");
        // Configuraciones de usabilidad para Swagger UI
        c.EnableDeepLinking(); // Permite enlaces directos a endpoints
        c.DisplayRequestDuration(); // Muestra el tiempo de las solicitudes
        c.EnableFilter(); // Habilita filtrado de endpoints
    });
}

// Middleware para redireccionar HTTP a HTTPS (seguridad)
app.UseHttpsRedirection();

// Middleware de autenticación (debe ir antes de autorización)
app.UseAuthentication();

// Middleware de autorización
app.UseAuthorization();

// Mapear los controladores a rutas
app.MapControllers();

// Ejecutar la aplicación
app.Run();