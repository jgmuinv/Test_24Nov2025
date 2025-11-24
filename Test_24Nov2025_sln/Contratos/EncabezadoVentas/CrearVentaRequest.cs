using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contratos.EncabezadoVentas;

public record CrearVentaRequest
{
    [Display(Name = "Vendedor")]
    [Required(ErrorMessage = "El vendedor es obligatorio")]
    public int Idvendedor { get; set; }
    
    public string? NombreVendedor { get; set; }
    
    // Mensajes de notificación
    public string? Mensaje { get; set; }
    public string? TipoMensaje { get; set; }
}