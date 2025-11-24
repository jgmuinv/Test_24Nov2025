// OBJETIVO: Gestionar la lógica de negocio en los registros 
using Contratos.General;
using Contratos.DetalleVentas;

namespace Aplicacion.Interfaces;

public interface IDetalleVentasService
{
    Task<ResultadoDto<IReadOnlyList<DetalleVentaDto?>>> ListarAsync(int? idventa, CancellationToken ct = default);

    Task<ResultadoDto<PaginadoDto<DetalleVentaDto?>>> ListarPaginadoAsync(int? idventa,
        int paginaActual, int registrosPorPagina,
        CancellationToken ct = default);

    Task<ResultadoDto<DetalleVentaDto?>> CrearAsync(CrearDetalleVentaDto dto, CancellationToken ct = default);
    Task<ResultadoDto<DetalleVentaDto?>> EditarAsync(int id, EditarDetalleVentaDto dto, CancellationToken ct = default);
    Task<ResultadoDto<bool?>> EliminarAsync(int id, CancellationToken ct = default);
}