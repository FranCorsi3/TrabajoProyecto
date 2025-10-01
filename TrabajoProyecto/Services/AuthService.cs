using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrabajoProyecto.Models;

namespace TrabajoProyecto.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        // Constructor que recibe la configuración mediante inyección de dependencias
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Método para autenticar usuarios y generar tokens JWT
        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
        {
            // Simulamos autenticación asíncrona (en un caso real, aquí se consultaría una base de datos)
            await Task.CompletedTask;

            // Verificar credenciales hardcodeadas (solo para demostración)
            // EN PRODUCCIÓN: Esto debe ser reemplazado por validación contra base de datos
            if (request.Username == "admin" && request.Password == "Admin123!")
            {
                // Crear el manejador de tokens JWT
                var tokenHandler = new JwtSecurityTokenHandler();

                // Obtener la clave secreta desde configuración o usar una por defecto para desarrollo
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "SuperSecretKeyForDevelopmentOnly123!");

                // Configurar las propiedades del token
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    // Definir la identidad del usuario (claims)
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, request.Username), // Claim del nombre de usuario
                        new Claim(ClaimTypes.Role, "Admin") // Claim del rol del usuario
                    }),
                    // El token expirará en 1 hora
                    Expires = DateTime.UtcNow.AddHours(1),
                    // Quién emite el token (obtenido de configuración)
                    Issuer = _configuration["Jwt:Issuer"],
                    // Para quién es válido el token (obtenido de configuración)
                    Audience = _configuration["Jwt:Audience"],
                    // Credenciales para firmar el token usando algoritmo HMAC-SHA256
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                // Crear el token JWT
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Convertir el token a string para enviarlo al cliente
                var tokenString = tokenHandler.WriteToken(token);

                // Devolver la respuesta con el token y su fecha de expiración
                return new LoginResponse
                {
                    Token = tokenString,
                    Expiration = tokenDescriptor.Expires.Value
                };
            }

            // Retornar null si las credenciales son incorrectas
            return null;
        }
    }
}