// OBJETIVO: Gestionar la lógica de negocio en los registros 
using Contratos.General;
using Contratos.EncabezadoVentas;

namespace Aplicacion.Interfaces;

public interface IEncabezadoVentasService
{
    Task<ResultadoDto<IReadOnlyList<EncabezadoVentaDto?>>> ListarAsync(int? idventa, int? idvendedor, CancellationToken ct = default);

    Task<ResultadoDto<PaginadoDto<EncabezadoVentaDto?>>> ListarPaginadoAsync(int? idvendedor,
        int paginaActual, int registrosPorPagina,
        CancellationToken ct = default);

    Task<ResultadoDto<EncabezadoVentaDto?>> CrearAsync(CrearEncabezadoVentaDto dto, CancellationToken ct = default);
    Task<ResultadoDto<EncabezadoVentaDto?>> EditarAsync(int id, EditarEncabezadoVentaDto dto, CancellationToken ct = default);
    Task<ResultadoDto<bool?>> EliminarAsync(int id, CancellationToken ct = default);
}