using System.Data;
using TrabajoProyecto.Models;

namespace TrabajoProyecto.Services
{
    // Interfaz que define el contrato para el servicio de base de datos
    // Contiene todas las operaciones CRUD para Clubes, Dirigentes y Socios
    public interface IDatabaseService
    {
        // Método para obtener una conexión a la base de datos
        IDbConnection GetConnection();

        // --- OPERACIONES PARA CLUBES ---

        // Obtener todos los clubes de la base de datos
        Task<IEnumerable<Club>> GetClubesAsync();

        // Obtener un club específico por su ID
        Task<Club?> GetClubByIdAsync(int id);

        // Crear un nuevo club y retornar el ID generado
        Task<int> CreateClubAsync(Club club);

        // Actualizar un club existente
        Task<bool> UpdateClubAsync(Club club);

        // Verificar si un club existe por su ID
        Task<bool> ClubExistsAsync(int id);

        // --- OPERACIONES PARA DIRIGENTES ---

        // Obtener todos los dirigentes
        Task<IEnumerable<Dirigente>> GetDirigentesAsync();

        // Obtener un dirigente específico por su ID
        Task<Dirigente?> GetDirigenteByIdAsync(int id);

        // Crear un nuevo dirigente y retornar el ID generado
        Task<int> CreateDirigenteAsync(Dirigente dirigente);

        // Actualizar un dirigente existente
        Task<bool> UpdateDirigenteAsync(Dirigente dirigente);

        // Verificar si un dirigente existe por su ID
        Task<bool> DirigenteExistsAsync(int id);

        // Verificar si un DNI ya existe en la tabla de dirigentes
        // excludeId permite excluir un ID específico (útil para actualizaciones)
        Task<bool> DniDirigenteExistsAsync(int dni, int? excludeId = null);

        // --- OPERACIONES PARA SOCIOS ---

        // Obtener todos los socios
        Task<IEnumerable<Socio>> GetSociosAsync();

        // Obtener un socio específico por su ID
        Task<Socio?> GetSocioByIdAsync(int id);

        // Crear un nuevo socio y retornar el ID generado
        Task<int> CreateSocioAsync(Socio socio);

        // Actualizar un socio existente
        Task<bool> UpdateSocioAsync(Socio socio);

        // Verificar si un socio existe por su ID
        Task<bool> SocioExistsAsync(int id);

        // Verificar si un DNI ya existe en la tabla de socios
        // excludeId permite excluir un ID específico (útil para actualizaciones)
        Task<bool> DniSocioExistsAsync(int dni, int? excludeId = null);
    }
}