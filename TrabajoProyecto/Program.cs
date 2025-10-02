// Importamos los namespaces necesarios para que funcione nuestra API
using Microsoft.AspNetCore.Authentication.JwtBearer;  // Para autenticación JWT
using Microsoft.IdentityModel.Tokens;                 // Para tokens de seguridad
using Microsoft.OpenApi.Models;                       // Para configurar Swagger/OpenAPI
using System.Text;                                    // Para trabajar con texto codificado
using TrabajoProyecto.Services;                      // Nuestros servicios personalizados

// Creamos el "constructor" de nuestra aplicación web
var builder = WebApplication.CreateBuilder(args);

// =============================================
// 🏗️  CONFIGURACIÓN DE SERVICIOS DE LA APLICACIÓN
// =============================================

// 🔹 Agregamos soporte para Controladores (MVC)
// Esto permite que nuestra API entienda los controladores que creamos
builder.Services.AddControllers();

// 🔹 Habilitamos Endpoints API y Explorer
// Esto permite que Swagger descubra automáticamente nuestros endpoints
builder.Services.AddEndpointsApiExplorer();

// =============================================
// 🎨  CONFIGURACIÓN DETALLADA DE SWAGGER
// =============================================

// 🔹 Configuramos Swagger/OpenAPI para documentar nuestra API
builder.Services.AddSwaggerGen(c =>
{
    // 📄 Información básica de nuestra API que aparece en Swagger
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "🏆 API Gestión de Clubes",          // Título de la API
        Version = "v1",                              // Versión de la API
        Description = "Sistema completo para gestión de clubes, dirigentes y socios",
        Contact = new OpenApiContact                 // Información de contacto
        {
            Name = "Createch",
            Email = "Createch@gmail.com"
        }
    });

    // =============================================
    // 🔐  CONFIGURACIÓN DE SEGURIDAD JWT EN SWAGGER
    // =============================================

    // 🔹 Definimos cómo Swagger manejará la autenticación
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa SOLO el token JWT. Ejemplo: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        Name = "Authorization",                       // Nombre del header
        In = ParameterLocation.Header,               // El token va en el header HTTP
        Type = SecuritySchemeType.Http,              // Tipo de esquema: HTTP (agrega "Bearer" automáticamente)
        Scheme = "bearer",                           // Esquema de autenticación (en minúscula)
        BearerFormat = "JWT"                         // Formato del token
    });

    // 🔹 Especificamos qué endpoints requieren autenticación
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            // Todos los endpoints que usen el esquema "Bearer" requerirán autenticación
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,  // Tipo de referencia
                    Id = "Bearer"                         // ID del esquema de seguridad
                }
            },
            Array.Empty<string>()  // Lista vacía = aplicar a todos los endpoints
        }
    });

    // =============================================
    // 📊  ORGANIZACIÓN Y ORDEN DE LOS ENDPOINTS
    // =============================================

    // 🔹 Ordenamos los endpoints para que Auth aparezca primero
    c.OrderActionsBy(apiDesc =>
    {
        // Si el endpoint es de Auth, le damos prioridad máxima ("0")
        if (apiDesc.RelativePath?.StartsWith("api/Auth") == true) return "0";

        // Los demás endpoints siguen en orden alfabético
        return apiDesc.RelativePath ?? "z";
    });

    // 🔹 Agrupamos los endpoints en categorías ordenadas
    c.TagActionsBy(api =>
    {
        // Categoría 0: Autenticación (aparece primera)
        if (api.RelativePath?.StartsWith("api/Auth") == true)
            return new[] { "0 - Autenticación" };

        // Categoría 1: Clubes
        if (api.RelativePath?.StartsWith("api/Clubes") == true)
            return new[] { "1 - Clubes" };

        // Categoría 2: Dirigentes
        if (api.RelativePath?.StartsWith("api/Dirigentes") == true)
            return new[] { "2 - Dirigentes" };

        // Categoría 3: Socios
        if (api.RelativePath?.StartsWith("api/Socios") == true)
            return new[] { "3 - Socios" };

        // Categoría 9: Otros (por si agregamos más endpoints)
        return new[] { "9 - Otros" };
    });
});

// =============================================
// 🔐  CONFIGURACIÓN DE AUTENTICACIÓN JWT
// =============================================

// 🔹 Configuramos la autenticación JWT en toda la aplicación
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Parámetros para validar los tokens JWT
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                    // Validar quién emitió el token
            ValidateAudience = true,                  // Validar para quién es el token
            ValidateLifetime = true,                  // Validar que no haya expirado
            ValidateIssuerSigningKey = true,          // Validar la firma del token

            // Datos válidos para nuestro token
            ValidIssuer = builder.Configuration["Jwt:Issuer"],        // Emisor válido
            ValidAudience = builder.Configuration["Jwt:Audience"],    // Audiencia válida

            // Clave secreta para verificar la firma del token
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"] ?? "SuperSecretKeyForDevelopmentOnly123!"
                )
            )
        };
    });

// 🔹 Habilitamos la autorización en la aplicación
builder.Services.AddAuthorization();

// =============================================
// ⚙️  REGISTRO DE NUESTROS SERVICIOS PERSONALIZADOS
// =============================================

// 🔹 Registramos nuestro servicio de base de datos
// "Scoped" significa: una instancia por cada request HTTP
builder.Services.AddScoped<IDatabaseService, DatabaseService>();

// 🔹 Registramos nuestro servicio de autenticación
builder.Services.AddScoped<IAuthService, AuthService>();

// =============================================
// 🚀  CONSTRUCCIÓN Y CONFIGURACIÓN DE LA APLICACIÓN
// =============================================

// 🔹 Construimos la aplicación con toda la configuración
var app = builder.Build();

// =============================================
// 🌐  CONFIGURACIÓN DEL PIPELINE DE REQUEST HTTP
// =============================================

// 🔹 Solo en desarrollo habilitamos Swagger (para producción se quitaría)
if (app.Environment.IsDevelopment())
{
    // Habilitamos Swagger para generar la documentación
    app.UseSwagger();

    // Habilitamos la interfaz web de Swagger
    app.UseSwaggerUI(c =>
    {
        // Especificamos el endpoint del archivo JSON de Swagger
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gestión de Clubes v1");

        // Título que aparece en la pestaña del navegador
        c.DocumentTitle = "🏆 API Gestión de Clubes";

        // ⚡ Características adicionales de Swagger UI
        c.EnableDeepLinking();        // Permite enlaces directos a secciones
        c.DisplayRequestDuration();   // Muestra cuánto tardó cada request
        c.EnableFilter();             // Habilita la barra de búsqueda
    });
}

// 🔹 Redirección automática a HTTPS (seguridad)
app.UseHttpsRedirection();

// 🔹 IMPORTANTE: El orden de estos middlewares es CRÍTICO
// 1. Primero la autenticación (saber QUIÉN es el usuario)
app.UseAuthentication();

// 2. Luego la autorización (saber QUÉ puede hacer el usuario)
app.UseAuthorization();

// 🔹 Mapeamos los controladores a las rutas HTTP
app.MapControllers();

// 🔹 FINAL: Ejecutamos la aplicación
app.Run();

/*
🎯 ¿QUÉ HACE ESTE ARCHIVO?
- Es el punto de entrada de nuestra aplicación
- Configura TODOS los servicios que usa nuestra API
- Define el pipeline de procesamiento de requests

🔧 FLUJO DE UN REQUEST:
1. Request HTTP llega al servidor
2. Swagger (si está en desarrollo)
3. Redirección HTTPS (seguridad)
4. Autenticación (verificar token JWT)
5. Autorización (verificar permisos)
6. Controladores (nuestra lógica)
7. Response HTTP al cliente

🚀 ENDPOINTS DISPONIBLES:
- GET    /api/Clubes          (Público)
- GET    /api/Clubes/{id}     (Público)  
- POST   /api/Clubes          (Protegido - necesita token)
- PUT    /api/Clubes/{id}     (Protegido - necesita token)
- POST   /api/Auth/login      (Público - para obtener token)

🔐 CREDENCIALES PARA PRUEBAS:
Usuario: admin
Contraseña: Admin123!

💡 CONSEJOS:
- El orden de los middlewares ES IMPORTANTE
- UseAuthentication() SIEMPRE antes de UseAuthorization()
- Swagger solo en desarrollo por seguridad
*/