namespace TrabajoProyecto.Models
{
    // Clase que representa un Club en el sistema
    public class Club
    {
        // Identificador único del club (clave primaria en la base de datos)
        public int ClubId { get; set; }

        // Nombre del club - se inicializa como string vacío para evitar null
        public string Nombre { get; set; } = string.Empty;

        // Cantidad total de socios que tiene el club
        public int CantidadSocios { get; set; }

        // Cantidad de títulos o trofeos que ha ganado el club
        public int CantidadTitulos { get; set; }

        // Fecha en que fue fundado el club
        public DateTime FechaFundacion { get; set; }

        // Dirección o ubicación física del estadio del club
        public string UbicacionEstadio { get; set; } = string.Empty;

        // Nombre oficial del estadio del club
        public string NombreEstadio { get; set; } = string.Empty;
    }
}