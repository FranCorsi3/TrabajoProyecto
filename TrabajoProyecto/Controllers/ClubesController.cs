using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrabajoProyecto.Models;
using TrabajoProyecto.Services;

namespace TrabajoProyecto.Controllers
{
    // Marca esta clase como un controlador de API
    [ApiController]
    // Define la ruta base para todos los endpoints: /api/clubes
    [Route("api/[controller]")]
    public class ClubesController : ControllerBase
    {
        // Servicio de base de datos inyectado mediante dependencias
        private readonly IDatabaseService _databaseService;

        // Logger para registrar eventos y errores
        private readonly ILogger<ClubesController> _logger;

        // Constructor que recibe las dependencias
        public ClubesController(IDatabaseService databaseService, ILogger<ClubesController> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        // GET: /api/clubes
        // Obtiene todos los clubes (público - no requiere autenticación)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Club>>> GetClubes()
        {
            try
            {
                var clubes = await _databaseService.GetClubesAsync();
                return Ok(clubes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clubes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: /api/clubes/{id}
        // Obtiene un club específico por su ID (público - no requiere autenticación)
        [HttpGet("{id}")]
        public async Task<ActionResult<Club>> GetClub(int id)
        {
            try
            {
                var club = await _databaseService.GetClubByIdAsync(id);

                // Si no se encuentra el club, retorna 404 Not Found
                if (club == null)
                {
                    return NotFound($"Club con ID {id} no encontrado");
                }

                return Ok(club);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener club con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // POST: /api/clubes
        // Crea un nuevo club (requiere autenticación JWT)
        [HttpPost]
        [Authorize] // Solo usuarios autenticados pueden crear clubes
        public async Task<ActionResult<Club>> CreateClub(Club club)
        {
            try
            {
                // Validaciones de negocio
                if (club.CantidadSocios < 0)
                {
                    return BadRequest("La cantidad de socios no puede ser negativa");
                }

                if (club.CantidadTitulos < 0)
                {
                    return BadRequest("La cantidad de títulos no puede ser negativa");
                }

                if (club.FechaFundacion > DateTime.Today)
                {
                    return BadRequest("La fecha de fundación no puede ser futura");
                }

                // Crear el club en la base de datos
                var clubId = await _databaseService.CreateClubAsync(club);

                // Asignar el ID generado automáticamente
                club.ClubId = clubId;

                // Retorna 201 Created con la ubicación del nuevo recurso
                return CreatedAtAction(nameof(GetClub), new { id = clubId }, club);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear club");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // PUT: /api/clubes/{id}
        // Actualiza un club existente (requiere autenticación JWT)
        [HttpPut("{id}")]
        [Authorize] // Solo usuarios autenticados pueden actualizar clubes
        public async Task<IActionResult> UpdateClub(int id, Club club)
        {
            try
            {
                // Verificar que el ID de la ruta coincide con el ID del objeto
                if (id != club.ClubId)
                {
                    return BadRequest("ID del club no coincide");
                }

                // Validaciones de negocio (iguales que en Create)
                if (club.CantidadSocios < 0)
                {
                    return BadRequest("La cantidad de socios no puede ser negativa");
                }

                if (club.CantidadTitulos < 0)
                {
                    return BadRequest("La cantidad de títulos no puede ser negativa");
                }

                if (club.FechaFundacion > DateTime.Today)
                {
                    return BadRequest("La fecha de fundación no puede ser futura");
                }

                // Verificar que el club existe antes de intentar actualizarlo
                if (!await _databaseService.ClubExistsAsync(id))
                {
                    return NotFound($"Club con ID {id} no encontrado");
                }

                // Ejecutar la actualización
                var updated = await _databaseService.UpdateClubAsync(club);

                // Si no se actualizó ningún registro, retorna error
                if (!updated)
                {
                    return StatusCode(500, "Error al actualizar el club");
                }

                // Retorna 204 No Content (actualización exitosa sin contenido en la respuesta)
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar club con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}