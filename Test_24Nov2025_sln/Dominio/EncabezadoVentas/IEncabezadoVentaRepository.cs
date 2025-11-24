// OBJETIVO: Gestionar el acceso a la base de datos de los encabezados de ventas 
using Contratos.General;

namespace Dominio.EncabezadoVentas;

public interface IEncabezadoVentaRepository
{
    // ==========================================
    // CRUD Básico
    // ==========================================
    Task<EncabezadoVenta?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<EncabezadoVenta>> ListarAsync(int? idventa, int? idvendedor, CancellationToken ct = default);
    Task CrearAsync(EncabezadoVenta obj, CancellationToken ct = default);
    Task ActualizarAsync(EncabezadoVenta obj, CancellationToken ct = default);
    Task EliminarAsync(int id, CancellationToken ct = default);
    
    // ==========================================
    // Paginación
    // ==========================================
    Task<PaginadoDto<EncabezadoVenta?>> ListarPaginadoAsync(
        int? idvendedor,
        int paginaActual,
        int registrosPorPagina,
        CancellationToken ct = default);
    
    // ==========================================
    // Verificación
    // ==========================================
    Task<bool> ExisteAsync(int id, CancellationToken ct = default);
    
    // ==========================================
    // Con Relaciones
    // ==========================================
    Task<EncabezadoVenta?> ObtenerConDetallesVentaAsync(int id, CancellationToken ct = default);
    
}