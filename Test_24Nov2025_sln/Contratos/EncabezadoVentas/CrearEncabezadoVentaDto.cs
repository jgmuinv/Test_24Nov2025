using System.ComponentModel.DataAnnotations;
using Contratos.DetalleVentas;

namespace Contratos.EncabezadoVentas;

public record CrearEncabezadoVentaDto
{
    public DateTime Fecha { get; set; } = DateTime.Now;
    
    [Required(ErrorMessage = "El vendedor es obligatorio")]
    public int Idvendedor { get; set; }
    
    // [Required(ErrorMessage = "El total es obligatorio")]
    // public decimal Total { get; set; }
    
    // Lista del detalle de la venta
    public List<DetalleVentaDto>? DetalleVenta { get; set; }
}