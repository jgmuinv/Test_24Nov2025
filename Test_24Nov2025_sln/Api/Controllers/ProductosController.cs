using Aplicacion.Interfaces;
using Contratos.General;
using Contratos.Productos;
using Dominio.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
//[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IProductosService _service;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(IProductosService service, ILogger<ProductosController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ============================
    // GET: /Productos/Listar
    // ============================
    [HttpGet]
    public async Task<ActionResult<ResultadoDto<IReadOnlyList<ProductoDto?>>>> Listar(int? idpro, string? Nombre, CancellationToken ct)
    {
        try
        {
            var lista = await _service.ListarAsync(idpro, Nombre, ct);
            return Ok(lista);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<IReadOnlyList<ProductoDto?>>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar productos");
            return StatusCode(500, ResultadoDto<IReadOnlyList<ProductoDto?>>.Failure("Error interno al listar productos"));
        }
    }
    
    // ============================
    // GET: /Productos/ListarPaginado
    // ============================
    [HttpGet]
    public async Task<ActionResult<ResultadoDto<PaginadoDto<ProductoDto?>>>> ListarPaginado(int? idpro, string? nombre, int paginaActual, int registrosPorPagina, CancellationToken ct)
    {
        try
        {
            var lista = await _service.ListarPaginadoAsync(idpro, nombre,paginaActual,registrosPorPagina, ct);
            return Ok(lista);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<PaginadoDto<ProductoDto?>>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar productos");
            return StatusCode(500, ResultadoDto<PaginadoDto<ProductoDto?>>.Failure("Error interno al listar productos"));
        }
    }

    // ============================
    // POST: /Productos/Crear
    // ============================
    [HttpPost]
    //[Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ResultadoDto<ProductoDto?>>> Crear([FromBody] CrearProductoRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var dto = new CrearProductoDto
            {
                Nombre = request.Nombre,
                Precio = request.Precio
            };

            var creado = await _service.CrearAsync(dto, ct);
            
            return Created(string.Empty, creado);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<ProductoDto?>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            return StatusCode(500,ResultadoDto<ProductoDto?>.Failure("Error interno al crear el producto"));
        }
    }

    // ============================
    // PUT: /Productos/Actualizar/5
    // ============================
    [HttpPut("{id:int}")]
    //[Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ResultadoDto<ProductoDto?>>> Actualizar(
        int id,
        [FromBody] ActualizarProductoRequest request,
        CancellationToken ct)
    {
        if (id != request.Id)
            return ResultadoDto<ProductoDto?>.Failure("El id de la ruta no coincide con el del cuerpo de la solicitud.");

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var dto = new EditarProductoDto
            {
                IdPro = request.Id,
                Nombre = request.Nombre,
                Precio = request.Precio
            };

            var actualizado = await _service.EditarAsync(id, dto, ct);

            return Ok(actualizado);
        }
        catch (DomainException de)
        {
            return BadRequest(ResultadoDto<ProductoDto?>.Failure(de.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto {Id}", id);
            return StatusCode(500,ResultadoDto<ProductoDto?>.Failure("Error interno al actualizar el producto"));
        }
    }

    // ============================
    // DELETE: /Productos/Eliminar/5
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
            _logger.LogError(ex, "Error al eliminar producto {Id}", id);
            return StatusCode(500,ResultadoDto<bool?>.Failure("Error interno al eliminar el producto"));
        }
    }
}