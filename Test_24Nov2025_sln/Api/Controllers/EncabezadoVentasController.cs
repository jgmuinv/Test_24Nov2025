using Aplicacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Dominio.Common;
using Contratos.EncabezadoVentas;
using Contratos.General;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
//[Authorize]
public class EncabezadoVentasController : ControllerBase
{
    private readonly IEncabezadoVentasService _service;
    private readonly ILogger<EncabezadoVentasController> _logger;

    public EncabezadoVentasController(IEncabezadoVentasService service, ILogger<EncabezadoVentasController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    // ============================
    // GET: /EncabezadoVentas/Listar
    // ============================
    [HttpGet]
    public async Task<ActionResult<ResultadoDto<IReadOnlyList<EncabezadoVentaDto?>>>> Listar(int? idventa,int? idvendedor, CancellationToken ct)
    {
        try
        {
            var lista = await _service.ListarAsync(idventa, idvendedor, ct);
            return Ok(lista);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<IReadOnlyList<EncabezadoVentaDto?>>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar registros");
            return StatusCode(500, ResultadoDto<IReadOnlyList<EncabezadoVentaDto?>>.Failure("Error interno al listar registros"));
        }
    }
    
    // ============================
    // GET: /EncabezadoVentas/ListarPaginado
    // ============================
    [HttpGet]
    public async Task<ActionResult<ResultadoDto<PaginadoDto<EncabezadoVentaDto?>>>> ListarPaginado(int? idvendedor, int paginaActual, int registrosPorPagina, CancellationToken ct)
    {
        try
        {
            var lista = await _service.ListarPaginadoAsync(idvendedor,paginaActual,registrosPorPagina, ct);
            return Ok(lista);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<PaginadoDto<EncabezadoVentaDto?>>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar registros");
            return StatusCode(500, ResultadoDto<PaginadoDto<EncabezadoVentaDto?>>.Failure("Error interno al listar registros"));
        }
    }

    // ============================
    // POST: /EncabezadoVentas/Crear
    // ============================
    [HttpPost]
    //[Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ResultadoDto<EncabezadoVentaDto?>>> Crear([FromBody] CrearEncabezadoVentaRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return 
        ResultadoDto<EncabezadoVentaDto?>.Failure(ModelState.Values.SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage).First());
        try
        {
            var dto = new CrearEncabezadoVentaDto
            {
                Idvendedor = request.IdVendedor,
                Fecha = request.Fecha,
                DetalleVenta = request.DetalleVenta
            };

            var creado = await _service.CrearAsync(dto, ct);
            
            return Created(string.Empty, creado);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<EncabezadoVentaDto?>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear registro");
            return StatusCode(500,ResultadoDto<EncabezadoVentaDto?>.Failure("Error interno al crear el registro"));
        }
    }

    // ============================
    // PUT: /EncabezadoVentas/Actualizar/5
    // ============================
    [HttpPut("{id:int}")]
    //[Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ResultadoDto<EncabezadoVentaDto?>>> Actualizar(
        int id,
        [FromBody] ActualizarEncabezadoVentaRequest request,
        CancellationToken ct)
    {
        if (id != request.IdVenta)
            return ResultadoDto<EncabezadoVentaDto?>.Failure("El id de la ruta no coincide con el del cuerpo de la solicitud.");

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var dto = new EditarEncabezadoVentaDto
            {
                Idventa = request.IdVenta,
                Idvendedor = request.IdVendedor
            };

            var actualizado = await _service.EditarAsync(id, dto, ct);

            return Ok(actualizado);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<EncabezadoVentaDto?>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar registro {Id}", id);
            return StatusCode(500,ResultadoDto<EncabezadoVentaDto?>.Failure("Error interno al actualizar el registro"));
        }
    }

    // ============================
    // DELETE: /EncabezadoVentas/Eliminar/5
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