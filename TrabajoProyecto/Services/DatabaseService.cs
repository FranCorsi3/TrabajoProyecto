using System.Data;
using System.Data.SqlClient;
using Dapper;
using TrabajoProyecto.Models;

namespace TrabajoProyecto.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;

        // Constructor que obtiene la cadena de conexión desde la configuración
        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string not found");
        }

        // Método para crear y devolver una conexión a la base de datos
        public IDbConnection GetConnection() => new SqlConnection(_connectionString);

        // --- OPERACIONES PARA CLUBES ---

        // Obtener todos los clubes de la base de datos
        public async Task<IEnumerable<Club>> GetClubesAsync()
        {
            using var connection = GetConnection();
            return await connection.QueryAsync<Club>("SELECT * FROM Club");
        }

        // Obtener un club específico por su ID
        public async Task<Club?> GetClubByIdAsync(int id)
        {
            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<Club>(
                "SELECT * FROM Club WHERE ClubId = @Id", new { Id = id });
        }

        // Crear un nuevo club y retornar el ID generado
        public async Task<int> CreateClubAsync(Club club)
        {
            using var connection = GetConnection();
            var sql = @"INSERT INTO Club (Nombre, CantidadSocios, CantidadTitulos, FechaFundacion, UbicacionEstadio, NombreEstadio)
                       VALUES (@Nombre, @CantidadSocios, @CantidadTitulos, @FechaFundacion, @UbicacionEstadio, @NombreEstadio);
                       SELECT CAST(SCOPE_IDENTITY() as int)"; // Retorna el ID autoincremental generado

            return await connection.ExecuteScalarAsync<int>(sql, club);
        }

        // Actualizar un club existente
        public async Task<bool> UpdateClubAsync(Club club)
        {
            using var connection = GetConnection();
            var sql = @"UPDATE Club SET 
                       Nombre = @Nombre, 
                       CantidadSocios = @CantidadSocios, 
                       CantidadTitulos = @CantidadTitulos, 
                       FechaFundacion = @FechaFundacion, 
                       UbicacionEstadio = @UbicacionEstadio, 
                       NombreEstadio = @NombreEstadio
                       WHERE ClubId = @ClubId";

            var affectedRows = await connection.ExecuteAsync(sql, club);
            return affectedRows > 0; // Retorna true si se actualizó al menos un registro
        }

        // Verificar si un club existe por su ID
        public async Task<bool> ClubExistsAsync(int id)
        {
            using var connection = GetConnection();
            return await connection.ExecuteScalarAsync<bool>(
                "SELECT COUNT(1) FROM Club WHERE ClubId = @Id", new { Id = id });
        }

        // --- OPERACIONES PARA DIRIGENTES ---

        // Obtener todos los dirigentes
        public async Task<IEnumerable<Dirigente>> GetDirigentesAsync()
        {
            using var connection = GetConnection();
            return await connection.QueryAsync<Dirigente>("SELECT * FROM Dirigente");
        }

        // Obtener un dirigente específico por su ID
        public async Task<Dirigente?> GetDirigenteByIdAsync(int id)
        {
            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<Dirigente>(
                "SELECT * FROM Dirigente WHERE DirigenteId = @Id", new { Id = id });
        }

        // Crear un nuevo dirigente y retornar el ID generado
        public async Task<int> CreateDirigenteAsync(Dirigente dirigente)
        {
            using var connection = GetConnection();
            var sql = @"INSERT INTO Dirigente (ClubId, Nombre, Apellido, FechaNacimiento, Rol, Dni)
                       VALUES (@ClubId, @Nombre, @Apellido, @FechaNacimiento, @Rol, @Dni);
                       SELECT CAST(SCOPE_IDENTITY() as int)";

            return await connection.ExecuteScalarAsync<int>(sql, dirigente);
        }

        // Actualizar un dirigente existente
        public async Task<bool> UpdateDirigenteAsync(Dirigente dirigente)
        {
            using var connection = GetConnection();
            var sql = @"UPDATE Dirigente SET 
                       ClubId = @ClubId,
                       Nombre = @Nombre, 
                       Apellido = @Apellido, 
                       FechaNacimiento = @FechaNacimiento, 
                       Rol = @Rol, 
                       Dni = @Dni
                       WHERE DirigenteId = @DirigenteId";

            var affectedRows = await connection.ExecuteAsync(sql, dirigente);
            return affectedRows > 0;
        }

        // Verificar si un dirigente existe por su ID
        public async Task<bool> DirigenteExistsAsync(int id)
        {
            using var connection = GetConnection();
            return await connection.ExecuteScalarAsync<bool>(
                "SELECT COUNT(1) FROM Dirigente WHERE DirigenteId = @Id", new { Id = id });
        }

        // Verificar si un DNI ya existe en la tabla de dirigentes
        // excludeId permite excluir un ID específico (útil para actualizaciones)
        public async Task<bool> DniDirigenteExistsAsync(int dni, int? excludeId = null)
        {
            using var connection = GetConnection();
            string sql;
            object parameters;

            if (excludeId.HasValue)
            {
                // Buscar DNI excluyendo un ID específico (para no contar el registro actual en actualizaciones)
                sql = "SELECT COUNT(1) FROM Dirigente WHERE Dni = @Dni AND DirigenteId != @ExcludeId";
                parameters = new { Dni = dni, ExcludeId = excludeId.Value };
            }
            else
            {
                // Buscar DNI sin exclusiones (para inserciones nuevas)
                sql = "SELECT COUNT(1) FROM Dirigente WHERE Dni = @Dni";
                parameters = new { Dni = dni };
            }

            return await connection.ExecuteScalarAsync<bool>(sql, parameters);
        }

        // --- OPERACIONES PARA SOCIOS ---

        // Obtener todos los socios
        public async Task<IEnumerable<Socio>> GetSociosAsync()
        {
            using var connection = GetConnection();
            return await connection.QueryAsync<Socio>("SELECT * FROM Socio");
        }

        // Obtener un socio específico por su ID
        public async Task<Socio?> GetSocioByIdAsync(int id)
        {
            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<Socio>(
                "SELECT * FROM Socio WHERE SocioId = @Id", new { Id = id });
        }

        // Crear un nuevo socio y retornar el ID generado
        public async Task<int> CreateSocioAsync(Socio socio)
        {
            using var connection = GetConnection();
            var sql = @"INSERT INTO Socio (ClubId, Nombre, Apellido, FechaNacimiento, FechaAsociado, Dni, CantidadAsistencias)
                       VALUES (@ClubId, @Nombre, @Apellido, @FechaNacimiento, @FechaAsociado, @Dni, @CantidadAsistencias);
                       SELECT CAST(SCOPE_IDENTITY() as int)";

            return await connection.ExecuteScalarAsync<int>(sql, socio);
        }

        // Actualizar un socio existente
        public async Task<bool> UpdateSocioAsync(Socio socio)
        {
            using var connection = GetConnection();
            var sql = @"UPDATE Socio SET 
                       ClubId = @ClubId,
                       Nombre = @Nombre, 
                       Apellido = @Apellido, 
                       FechaNacimiento = @FechaNacimiento, 
                       FechaAsociado = @FechaAsociado, 
                       Dni = @Dni, 
                       CantidadAsistencias = @CantidadAsistencias
                       WHERE SocioId = @SocioId";

            var affectedRows = await connection.ExecuteAsync(sql, socio);
            return affectedRows > 0;
        }

        // Verificar si un socio existe por su ID
        public async Task<bool> SocioExistsAsync(int id)
        {
            using var connection = GetConnection();
            return await connection.ExecuteScalarAsync<bool>(
                "SELECT COUNT(1) FROM Socio WHERE SocioId = @Id", new { Id = id });
        }

        // Verificar si un DNI ya existe en la tabla de socios
        // excludeId permite excluir un ID específico (útil para actualizaciones)
        public async Task<bool> DniSocioExistsAsync(int dni, int? excludeId = null)
        {
            using var connection = GetConnection();
            string sql;
            object parameters;

            if (excludeId.HasValue)
            {
                // Buscar DNI excluyendo un ID específico
                sql = "SELECT COUNT(1) FROM Socio WHERE Dni = @Dni AND SocioId != @ExcludeId";
                parameters = new { Dni = dni, ExcludeId = excludeId.Value };
            }
            else
            {
                // Buscar DNI sin exclusiones
                sql = "SELECT COUNT(1) FROM Socio WHERE Dni = @Dni";
                parameters = new { Dni = dni };
            }

            return await connection.ExecuteScalarAsync<bool>(sql, parameters);
        }
    }
}