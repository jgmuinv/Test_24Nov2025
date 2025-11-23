using System.ComponentModel.DataAnnotations;

namespace Contratos.EncabezadoVentas;

public record CrearEncabezadoVentaDto
{
    public DateTime Fecha { get; set; } = DateTime.Now;
    
    [Required(ErrorMessage = "El vendedor es obligatorio")]
    public int Idvendedor { get; set; }
    
    [Required(ErrorMessage = "El total es obligatorio")]
    public decimal Total { get; set; }
}