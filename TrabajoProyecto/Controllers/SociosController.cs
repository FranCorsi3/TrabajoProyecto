using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrabajoProyecto.Models;
using TrabajoProyecto.Services;

namespace TrabajoProyecto.Controllers
{
    // Controlador para gestionar las operaciones CRUD de Socios
    [ApiController]
    // Define la ruta base para todos los endpoints: /api/socios
    [Route("api/[controller]")]
    public class SociosController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<SociosController> _logger;

        public SociosController(IDatabaseService databaseService, ILogger<SociosController> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        // GET: /api/socios
        // Obtiene todos los socios (público - no requiere autenticación)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Socio>>> GetSocios()
        {
            try
            {
                var socios = await _databaseService.GetSociosAsync();
                return Ok(socios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener socios");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: /api/socios/{id}
        // Obtiene un socio específico por su ID (público - no requiere autenticación)
        [HttpGet("{id}")]
        public async Task<ActionResult<Socio>> GetSocio(int id)
        {
            try
            {
                var socio = await _databaseService.GetSocioByIdAsync(id);

                // Si no se encuentra el socio, retorna 404 Not Found
                if (socio == null)
                {
                    return NotFound($"Socio con ID {id} no encontrado");
                }

                return Ok(socio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener socio con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // POST: /api/socios
        // Crea un nuevo socio (requiere autenticación JWT)
        [HttpPost]
        [Authorize] // Solo usuarios autenticados pueden crear socios
        public async Task<ActionResult<Socio>> CreateSocio(Socio socio)
        {
            try
            {
                // Validaciones de negocio

                // Verificar que el club al que se quiere asignar el socio existe
                if (!await _databaseService.ClubExistsAsync(socio.ClubId))
                {
                    return BadRequest("El ClubId especificado no existe");
                }

                // Verificar que no exista otro socio con el mismo DNI
                if (await _databaseService.DniSocioExistsAsync(socio.Dni))
                {
                    return BadRequest("Ya existe un socio con este DNI");
                }

                // Validar que la cantidad de asistencias no sea negativa
                if (socio.CantidadAsistencias < 0)
                {
                    return BadRequest("La cantidad de asistencias no puede ser negativa");
                }

                // Validar lógica de fechas: no puede asociarse antes de nacer
                if (socio.FechaAsociado < socio.FechaNacimiento)
                {
                    return BadRequest("La fecha de asociado no puede ser anterior a la fecha de nacimiento");
                }

                // Crear el socio en la base de datos
                var socioId = await _databaseService.CreateSocioAsync(socio);

                // Asignar el ID generado automáticamente
                socio.SocioId = socioId;

                // Retorna 201 Created con la ubicación del nuevo recurso
                return CreatedAtAction(nameof(GetSocio), new { id = socioId }, socio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear socio");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // PUT: /api/socios/{id}
        // Actualiza un socio existente (requiere autenticación JWT)
        [HttpPut("{id}")]
        [Authorize] // Solo usuarios autenticados pueden actualizar socios
        public async Task<IActionResult> UpdateSocio(int id, Socio socio)
        {
            try
            {
                // Verificar que el ID de la ruta coincide con el ID del objeto
                if (id != socio.SocioId)
                {
                    return BadRequest("ID del socio no coincide");
                }

                // Validaciones de negocio

                // Verificar que el club existe
                if (!await _databaseService.ClubExistsAsync(socio.ClubId))
                {
                    return BadRequest("El ClubId especificado no existe");
                }

                // Verificar que no exista OTRO socio con el mismo DNI (excluyendo el actual)
                if (await _databaseService.DniSocioExistsAsync(socio.Dni, id))
                {
                    return BadRequest("Ya existe otro socio con este DNI");
                }

                // Validar que la cantidad de asistencias no sea negativa
                if (socio.CantidadAsistencias < 0)
                {
                    return BadRequest("La cantidad de asistencias no puede ser negativa");
                }

                // Validar lógica de fechas: no puede asociarse antes de nacer
                if (socio.FechaAsociado < socio.FechaNacimiento)
                {
                    return BadRequest("La fecha de asociado no puede ser anterior a la fecha de nacimiento");
                }

                // Verificar que el socio existe antes de intentar actualizarlo
                if (!await _databaseService.SocioExistsAsync(id))
                {
                    return NotFound($"Socio con ID {id} no encontrado");
                }

                // Ejecutar la actualización
                var updated = await _databaseService.UpdateSocioAsync(socio);

                // Si no se actualizó ningún registro, retorna error
                if (!updated)
                {
                    return StatusCode(500, "Error al actualizar el socio");
                }

                // Retorna 204 No Content (actualización exitosa sin contenido en la respuesta)
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar socio con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}