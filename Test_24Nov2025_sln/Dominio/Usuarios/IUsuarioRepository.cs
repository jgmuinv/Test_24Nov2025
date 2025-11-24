namespace Dominio.Usuarios;

public interface IUsuarioRepository
{
    // ==========================================
    // CRUD Básico
    // ==========================================
    // Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    // Task<IReadOnlyList<Producto>> ListarAsync(int? idpro, string? nombre, CancellationToken ct = default);
    Task<IReadOnlyList<Usuario>> ListarNombresAsync(int? idus, string? usuario, string? nombre, CancellationToken ct = default);
    // Task CrearAsync(Producto producto, CancellationToken ct = default);
    // Task ActualizarAsync(Producto producto, CancellationToken ct = default);
    // Task EliminarAsync(int id, CancellationToken ct = default);
}