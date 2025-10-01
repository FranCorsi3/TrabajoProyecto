namespace TrabajoProyecto.Models
{
    // Clase que representa un Socio en el sistema
    // Un socio es una persona que está asociada a un club específico
    public class Socio
    {
        // Identificador único del socio (clave primaria en la base de datos)
        public int SocioId { get; set; }

        // Identificador del club al que pertenece el socio (clave foránea)
        // Relaciona este socio con un club específico
        public int ClubId { get; set; }

        // Nombre del socio - se inicializa como string vacío para evitar null
        public string Nombre { get; set; } = string.Empty;

        // Apellido del socio - se inicializa como string vacío para evitar null
        public string Apellido { get; set; } = string.Empty;

        // Fecha de nacimiento del socio
        public DateTime FechaNacimiento { get; set; }

        // Fecha en que el socio se asoció al club
        public DateTime FechaAsociado { get; set; }

        // Documento Nacional de Identidad - número único de identificación
        // Se valida que no se repita entre socios
        public int Dni { get; set; }

        // Número de asistencias registradas del socio a eventos del club
        // Puede usarse para calcular beneficios o estadísticas
        public int CantidadAsistencias { get; set; }
    }
}