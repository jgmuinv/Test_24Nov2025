using System.ComponentModel.DataAnnotations;

namespace Contratos.EncabezadoVentas;

public class CrearEncabezadoVentaRequest
{
    public DateTime Fecha { get; set; } = DateTime.Now;
    
    [Required(ErrorMessage = "El vendedor es obligatorio")]
    public int IdVendedor { get; set; }
    
    [Required(ErrorMessage = "El total es obligatorio")]
    [Range(0.01, 99999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 99,999,999.99")]

    public decimal Total { get; set; }
    
    // Mensajes de notificación
    public string? Mensaje { get; set; }
    public string? TipoMensaje { get; set; }
}