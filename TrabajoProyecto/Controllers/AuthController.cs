using Microsoft.AspNetCore.Mvc;
using TrabajoProyecto.Models;
using TrabajoProyecto.Services;

namespace TrabajoProyecto.Controllers
{
    // Marca esta clase como un controlador de API que proporciona respuestas HTTP
    [ApiController]
    // Define la ruta base para todos los endpoints en este controlador: /api/auth
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Servicio de autenticación inyectado mediante dependencias
        private readonly IAuthService _authService;

        // Logger para registrar eventos y errores
        private readonly ILogger<AuthController> _logger;

        // Constructor que recibe las dependencias mediante inyección
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // Endpoint POST para autenticar usuarios
        // Ruta completa: POST /api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            try
            {
                // Validar que se proporcionaron tanto username como password
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    // Retorna 400 Bad Request si faltan credenciales
                    return BadRequest("Username y Password son requeridos");
                }

                // Llamar al servicio de autenticación para validar credenciales
                var response = await _authService.AuthenticateAsync(request);

                // Si las credenciales son inválidas, el servicio retorna null
                if (response == null)
                {
                    // Retorna 401 Unauthorized con mensaje descriptivo
                    return Unauthorized("Credenciales inválidas");
                }

                // Retorna 200 OK con el token JWT y fecha de expiración
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Registrar el error en el log para debugging
                _logger.LogError(ex, "Error durante el login");

                // Retorna 500 Internal Server Error con mensaje genérico
                // (no se revelan detalles internos por seguridad)
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}