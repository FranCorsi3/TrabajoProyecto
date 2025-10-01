namespace TrabajoProyecto.Models
{
    // Clase que representa la solicitud de login de un usuario
    // Contiene las credenciales necesarias para autenticarse en el sistema
    public class LoginRequest
    {
        // Nombre de usuario para el login - se inicializa como string vacío para evitar null
        public string Username { get; set; } = string.Empty;

        // Contraseña del usuario para el login - se inicializa como string vacío para evitar null
        // NOTA: En una aplicación real, la contraseña debería encriptarse antes de enviarse
        public string Password { get; set; } = string.Empty;
    }
}