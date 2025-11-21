// OBJETIVO: Gestionar el acceso a la base de datos de los productos 

using Contratos.General;

namespace Dominio.Productos;

public interface IProductoRepository
{
    // ==========================================
    // CRUD Básico
    // ==========================================
    Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Producto>> ListarAsync(int? idpro, string? nombre, CancellationToken ct = default);
    Task CrearAsync(Producto producto, CancellationToken ct = default);
    Task ActualizarAsync(Producto producto, CancellationToken ct = default);
    Task EliminarAsync(int id, CancellationToken ct = default);

    // ==========================================
    // Paginación
    // ==========================================
    Task<int> ContarTodos(CancellationToken ct = default);
    Task<List<Producto>> ObtenerPaginado(int saltar, int tomar, CancellationToken ct = default);

    // ==========================================
    // Búsqueda con Filtros
    // ==========================================
    Task<PaginadoDto<Producto>> BuscarPaginadoAsync(int? idpro,
        string? nombre,
        int paginaActual,
        int registrosPorPagina,
        CancellationToken ct = default);

    // ==========================================
    // Verificación
    // ==========================================
    Task<bool> ExisteAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteNombreAsync(string nombre, CancellationToken ct = default);
    Task<bool> ExisteNombreAsync(string nombre, int idExcluir, CancellationToken ct = default);

    // ==========================================
    // Con Relaciones (opcional)
    // ==========================================
    Task<Producto?> ObtenerConDetallesVentaAsync(int id, CancellationToken ct = default);

    // ==========================================
    // Operaciones en Lote (opcional)
    // ==========================================
    Task<List<Producto>> ObtenerPorIdsAsync(List<int> ids, CancellationToken ct = default);
    Task ActualizarPreciosAsync(Dictionary<int, decimal> cambiosPrecios, CancellationToken ct = default);
}