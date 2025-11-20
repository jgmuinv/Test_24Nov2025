namespace Aplicacion.Interfaces;

public interface IProductosService
{
    // OBJETIVO: Gestionar la lógica de negocio en los productos 
    
    Task<IReadOnlyList<ProductoDto>> ListarAsync(string? nombre, string? descripcion, bool eliminado, CancellationToken ct = default);
    Task<ProductoDto> CrearAsync(CrearProductoDto dto, CancellationToken ct = default);
    Task<ProductoDto?> EditarAsync(int id, EditarProductoDto dto, CancellationToken ct = default);
    Task<bool> EliminarAsync(int id, CancellationToken ct = default);
}