using Contratos.General;
using Contratos.Productos;
using Contratos.Usuarios;

namespace Aplicacion.Interfaces;

public interface IUsuariosService
{
    Task<ResultadoDto<IReadOnlyList<NombreUsuariosDto?>>> ListarNombresAsync(int? idus, string? usuario, string? nombre, CancellationToken ct = default);

    // Task<ResultadoDto<PaginadoDto<ProductoDto?>>> ListarPaginadoAsync(int? idpro, string? nombre,
    //     int paginaActual, int registrosPorPagina, CancellationToken ct = default);
    //
    // Task<ResultadoDto<ProductoDto?>> CrearAsync(CrearProductoDto dto, CancellationToken ct = default);
    // Task<ResultadoDto<ProductoDto?>> EditarAsync(int id, EditarProductoDto dto, CancellationToken ct = default);
    // Task<ResultadoDto<bool?>> EliminarAsync(int id, CancellationToken ct = default);
}