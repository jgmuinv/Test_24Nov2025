namespace Contratos.Productos;

/// <summary>
/// DTO para representar un producto
/// </summary>
public record ProductoDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public decimal Precio { get; init; }
}