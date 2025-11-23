using System.ComponentModel.DataAnnotations;

namespace Contratos.Productos;

/// <summary>
/// DTO para representar un producto
/// </summary>
public record ProductoDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Precio { get; init; }
}