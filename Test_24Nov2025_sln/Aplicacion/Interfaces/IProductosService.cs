using Contratos.General;
using Contratos.Productos;
namespace Aplicacion.Interfaces;

public interface IProductosService
{
    // OBJETIVO: Gestionar la lógica de negocio en los productos 
    
    Task<ResultadoDto<IReadOnlyList<ProductoDto?>>> ListarAsync(int? idpro, string? nombre, CancellationToken ct = default);

    Task<ResultadoDto<PaginadoDto<ProductoDto?>>> ListarPaginadoAsync(int? idpro, string? nombre,
        int paginaActual, int registrosPorPagina, CancellationToken ct = default);

    Task<ResultadoDto<ProductoDto?>> CrearAsync(CrearProductoDto dto, CancellationToken ct = default);
    Task<ResultadoDto<ProductoDto?>> EditarAsync(int id, EditarProductoDto dto, CancellationToken ct = default);
    Task<ResultadoDto<bool?>> EliminarAsync(int id, CancellationToken ct = default);
}