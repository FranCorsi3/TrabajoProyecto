using TrabajoProyecto.Models;

namespace TrabajoProyecto.Services
{
    // Interfaz que define el contrato para el servicio de autenticación
    public interface IAuthService
    {
        // Método para autenticar un usuario basado en sus credenciales
        // Recibe un objeto LoginRequest con username y password
        // Retorna un LoginResponse con el token JWT si la autenticación es exitosa, o null si falla
        Task<LoginResponse?> AuthenticateAsync(LoginRequest request);
    }
}