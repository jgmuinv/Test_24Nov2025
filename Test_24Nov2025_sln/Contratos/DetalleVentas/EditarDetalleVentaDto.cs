using System.ComponentModel.DataAnnotations;

namespace Contratos.DetalleVentas;

public record EditarDetalleVentaDto
{
    [Required(ErrorMessage = "El Id es obligatorio")]
    public int Idde { get; set; }
    
    public DateTime Fecha { get; set; }
    
    [Required(ErrorMessage = "El id de la venta es obligatorio")]
    public int Idventa { get; set; }
    
    [Required(ErrorMessage = "El id del producto es obligatorio")]
    public int Idpro { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria")]
    public decimal Cantidad { get; set; }

    public decimal Precio { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }
};