namespace Contratos.Productos;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO para editar un producto existente
/// </summary>
public record EditarProductoDto
{
    [Required(ErrorMessage = "El Id es obligatorio")]
    public Int32 IdPro { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; init; } = string.Empty;

    [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999,999.99")]
    public decimal? Precio { get; init; }
}