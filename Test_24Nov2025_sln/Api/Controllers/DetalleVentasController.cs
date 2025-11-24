using Aplicacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Dominio.Common;
using Contratos.DetalleVentas;
using Contratos.General;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
//[Authorize]

public class DetalleVentasController : ControllerBase
{
    private readonly IDetalleVentasService _service;
    private readonly ILogger<DetalleVentasController> _logger;

    public DetalleVentasController(IDetalleVentasService service, ILogger<DetalleVentasController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    // ============================
    // GET: /DetalleVentas/Listar
    // ============================
    [HttpGet]
    public async Task<ActionResult<ResultadoDto<IReadOnlyList<DetalleVentaDto?>>>> Listar(int? idventa, CancellationToken ct)
    {
        try
        {
            var lista = await _service.ListarAsync(idventa, ct);
            return Ok(lista);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<IReadOnlyList<DetalleVentaDto?>>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar registros");
            return StatusCode(500, ResultadoDto<IReadOnlyList<DetalleVentaDto?>>.Failure("Error interno al listar registros"));
        }
    }
    
    // ============================
    // GET: /DetalleVentas/ListarPaginado
    // ============================
    [HttpGet]
    public async Task<ActionResult<ResultadoDto<PaginadoDto<DetalleVentaDto?>>>> ListarPaginado(int? idventa, int paginaActual, int registrosPorPagina, CancellationToken ct)
    {
        try
        {
            var lista = await _service.ListarPaginadoAsync(idventa,paginaActual,registrosPorPagina, ct);
            return Ok(lista);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<PaginadoDto<DetalleVentaDto?>>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar registros");
            return StatusCode(500, ResultadoDto<PaginadoDto<DetalleVentaDto?>>.Failure("Error interno al listar registros"));
        }
    }

    // ============================
    // POST: /DetalleVentas/Crear
    // ============================
    [HttpPost]
    //[Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ResultadoDto<DetalleVentaDto?>>> Crear([FromBody] CrearDetalleVentaRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return 
        ResultadoDto<DetalleVentaDto?>.Failure(ModelState.Values.SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage).First());
        try
        {
            var dto = new CrearDetalleVentaDto
            {
                Idventa = request.Idventa,
                Fecha = request.Fecha,
                Idpro = request.Idpro,
                Cantidad = request.Cantidad,
                Precio = request.Precio,
                Total = request.Total,
                Iva = request.Iva
            };

            var creado = await _service.CrearAsync(dto, ct);
            
            return Created(string.Empty, creado);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<DetalleVentaDto?>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear registro");
            return StatusCode(500,ResultadoDto<DetalleVentaDto?>.Failure("Error interno al crear el registro"));
        }
    }

    // ============================
    // PUT: /DetalleVentas/Actualizar/5
    // ============================
    [HttpPut("{id:int}")]
    //[Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ResultadoDto<DetalleVentaDto?>>> Actualizar(
        int id,
        [FromBody] ActualizarDetalleVentaRequest request,
        CancellationToken ct)
    {
        if (id != request.Idde)
            return ResultadoDto<DetalleVentaDto?>.Failure("El id de la ruta no coincide con el del cuerpo de la solicitud.");

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var dto = new EditarDetalleVentaDto
            {
                Idde = request.Idde,
                Idventa = request.Idventa,
                Fecha = request.Fecha,
                Idpro = request.Idpro,
                Cantidad = request.Cantidad,
                Precio = request.Precio,
                Total = request.Total,
                Iva = request.Iva
            };

            var actualizado = await _service.EditarAsync(id, dto, ct);

            return Ok(actualizado);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<DetalleVentaDto?>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar registro {Id}", id);
            return StatusCode(500,ResultadoDto<DetalleVentaDto?>.Failure("Error interno al actualizar el registro"));
        }
    }

    // ============================
    // DELETE: /DetalleVentas/Eliminar/5
    // ============================
    [HttpDelete("{id:int}")]
    //[Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ResultadoDto<bool?>>> Eliminar(int id, CancellationToken ct)
    {
        try
        {
            var eliminado = await _service.EliminarAsync(id, ct);
            
            return ResultadoDto<bool?>.Success(true);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<bool?>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar registro {Id}", id);
            return StatusCode(500,ResultadoDto<bool?>.Failure("Error interno al eliminar el registro"));
        }
    }
}