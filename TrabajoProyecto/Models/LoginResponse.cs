namespace TrabajoProyecto.Models
{
    // Clase que representa la respuesta del servidor después de un login exitoso
    // Contiene el token de autenticación y información relacionada
    public class LoginResponse
    {
        // Token JWT (JSON Web Token) generado para el usuario autenticado
        // Este token debe incluirse en el header Authorization de las solicitudes posteriores
        // Formato: "Bearer {token}"
        public string Token { get; set; } = string.Empty;

        // Fecha y hora en la que el token expirará y dejará de ser válido
        // El cliente debe renovar el token antes de esta fecha
        public DateTime Expiration { get; set; }
    }
}