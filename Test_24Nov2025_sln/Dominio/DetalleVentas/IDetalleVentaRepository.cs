// OBJETIVO: Gestionar el acceso a la base de datos de los encabezados de ventas 
using Contratos.General;
namespace Dominio.DetalleVentas;

public interface IDetalleVentaRepository
{
    // ==========================================
    // CRUD Básico
    // ==========================================
    Task<DetalleVenta?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<DetalleVenta>> ListarAsync(int? idventa, CancellationToken ct = default);
    Task CrearAsync(DetalleVenta obj, CancellationToken ct = default);
    Task ActualizarAsync(DetalleVenta obj, CancellationToken ct = default);
    Task EliminarAsync(int id, CancellationToken ct = default);
    
    // ==========================================
    // Paginación
    // ==========================================
    Task<PaginadoDto<DetalleVenta?>> ListarPaginadoAsync(
        int? idventa,
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
    Task<DetalleVenta?> ObtenerConProductosAsync(int id, CancellationToken ct = default);
}