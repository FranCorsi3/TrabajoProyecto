using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrabajoProyecto.Models;
using TrabajoProyecto.Services;

namespace TrabajoProyecto.Controllers
{
    // Controlador para gestionar las operaciones CRUD de Dirigentes
    [ApiController]
    // Define la ruta base para todos los endpoints: /api/dirigentes
    [Route("api/[controller]")]
    public class DirigentesController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<DirigentesController> _logger;

        public DirigentesController(IDatabaseService databaseService, ILogger<DirigentesController> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        // GET: /api/dirigentes
        // Obtiene todos los dirigentes (público - no requiere autenticación)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dirigente>>> GetDirigentes()
        {
            try
            {
                var dirigentes = await _databaseService.GetDirigentesAsync();
                return Ok(dirigentes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener dirigentes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: /api/dirigentes/{id}
        // Obtiene un dirigente específico por su ID (público - no requiere autenticación)
        [HttpGet("{id}")]
        public async Task<ActionResult<Dirigente>> GetDirigente(int id)
        {
            try
            {
                var dirigente = await _databaseService.GetDirigenteByIdAsync(id);

                // Si no se encuentra el dirigente, retorna 404 Not Found
                if (dirigente == null)
                {
                    return NotFound($"Dirigente con ID {id} no encontrado");
                }

                return Ok(dirigente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener dirigente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // POST: /api/dirigentes
        // Crea un nuevo dirigente (requiere autenticación JWT)
        [HttpPost]
        [Authorize] // Solo usuarios autenticados pueden crear dirigentes
        public async Task<ActionResult<Dirigente>> CreateDirigente(Dirigente dirigente)
        {
            try
            {
                // Validaciones de negocio

                // Verificar que el club al que se quiere asignar el dirigente existe
                if (!await _databaseService.ClubExistsAsync(dirigente.ClubId))
                {
                    return BadRequest("El ClubId especificado no existe");
                }

                // Verificar que no exista otro dirigente con el mismo DNI
                if (await _databaseService.DniDirigenteExistsAsync(dirigente.Dni))
                {
                    return BadRequest("Ya existe un dirigente con este DNI");
                }

                // Validar que el dirigente sea mayor de edad (18 años)
                if (dirigente.FechaNacimiento > DateTime.Today.AddYears(-18))
                {
                    return BadRequest("El dirigente debe ser mayor de edad");
                }

                // Crear el dirigente en la base de datos
                var dirigenteId = await _databaseService.CreateDirigenteAsync(dirigente);

                // Asignar el ID generado automáticamente
                dirigente.DirigenteId = dirigenteId;

                // Retorna 201 Created con la ubicación del nuevo recurso
                return CreatedAtAction(nameof(GetDirigente), new { id = dirigenteId }, dirigente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear dirigente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // PUT: /api/dirigentes/{id}
        // Actualiza un dirigente existente (requiere autenticación JWT)
        [HttpPut("{id}")]
        [Authorize] // Solo usuarios autenticados pueden actualizar dirigentes
        public async Task<IActionResult> UpdateDirigente(int id, Dirigente dirigente)
        {
            try
            {
                // Verificar que el ID de la ruta coincide con el ID del objeto
                if (id != dirigente.DirigenteId)
                {
                    return BadRequest("ID del dirigente no coincide");
                }

                // Validaciones de negocio

                // Verificar que el club existe
                if (!await _databaseService.ClubExistsAsync(dirigente.ClubId))
                {
                    return BadRequest("El ClubId especificado no existe");
                }

                // Verificar que no exista OTRO dirigente con el mismo DNI (excluyendo el actual)
                if (await _databaseService.DniDirigenteExistsAsync(dirigente.Dni, id))
                {
                    return BadRequest("Ya existe otro dirigente con este DNI");
                }

                // Validar que el dirigente sea mayor de edad
                if (dirigente.FechaNacimiento > DateTime.Today.AddYears(-18))
                {
                    return BadRequest("El dirigente debe ser mayor de edad");
                }

                // Verificar que el dirigente existe antes de intentar actualizarlo
                if (!await _databaseService.DirigenteExistsAsync(id))
                {
                    return NotFound($"Dirigente con ID {id} no encontrado");
                }

                // Ejecutar la actualización
                var updated = await _databaseService.UpdateDirigenteAsync(dirigente);

                // Si no se actualizó ningún registro, retorna error
                if (!updated)
                {
                    return StatusCode(500, "Error al actualizar el dirigente");
                }

                // Retorna 204 No Content (actualización exitosa sin contenido en la respuesta)
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar dirigente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}