namespace TrabajoProyecto.Models
{
    // Clase que representa un Dirigente en el sistema
    // Un dirigente es una persona que ocupa un cargo directivo en un club
    public class Dirigente
    {
        // Identificador único del dirigente (clave primaria en la base de datos)
        public int DirigenteId { get; set; }

        // Identificador del club al que pertenece el dirigente (clave foránea)
        // Relaciona este dirigente con un club específico
        public int ClubId { get; set; }

        // Nombre del dirigente - se inicializa como string vacío para evitar null
        public string Nombre { get; set; } = string.Empty;

        // Apellido del dirigente - se inicializa como string vacío para evitar null
        public string Apellido { get; set; } = string.Empty;

        // Fecha de nacimiento del dirigente
        public DateTime FechaNacimiento { get; set; }

        // Cargo o posición que ocupa el dirigente en el club
        // Ejemplos: "Presidente", "Vicepresidente", "Tesorero", etc.
        public string Rol { get; set; } = string.Empty;

        // Documento Nacional de Identidad - número único de identificación
        // Se valida que no se repita entre dirigentes
        public int Dni { get; set; }
    }
}