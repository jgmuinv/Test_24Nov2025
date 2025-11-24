using System.ComponentModel.DataAnnotations;

namespace Contratos.DetalleVentas;

public class CrearDetalleVentaRequest
{
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
    
    // Mensajes de notificación
    public string? Mensaje { get; set; }
    public string? TipoMensaje { get; set; }
}