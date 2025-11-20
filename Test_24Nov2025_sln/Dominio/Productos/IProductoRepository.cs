namespace Dominio.Productos;

public interface IProductoRepository
{
    // OBJETIVO: Gestionar el acceso a la base de datos de los productos 
    
    Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Producto>> ListarAsync(CancellationToken ct = default);
    Task CrearAsync(Producto producto, CancellationToken ct = default);
    Task ActualizarAsync(Producto producto, CancellationToken ct = default);
    Task EliminarAsync(int id, CancellationToken ct = default);
}