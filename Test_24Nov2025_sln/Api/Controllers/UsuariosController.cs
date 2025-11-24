using Aplicacion.Interfaces;
using Contratos.General;
using Contratos.Productos;
using Contratos.Usuarios;
using Dominio.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
//[Authorize]

public class UsuariosController : ControllerBase
{
    private readonly IUsuariosService _service;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IUsuariosService service, ILogger<UsuariosController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    // ============================
    // GET: /Usuarios/ListarNombres
    // ============================
    [HttpGet]
    public async Task<ActionResult<ResultadoDto<IReadOnlyList<NombreUsuariosDto?>>>> ListarNombres(int? idus, string? usuario, string? nombre, CancellationToken ct)
    {
        try
        {
            var lista = await _service.ListarNombresAsync(idus, usuario ,nombre, ct);
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
}