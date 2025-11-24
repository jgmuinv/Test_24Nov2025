using System.ComponentModel.DataAnnotations;
using Contratos.DetalleVentas;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Contratos.EncabezadoVentas;

public class ActualizarEncabezadoVentaRequest
{
    public int IdVenta { get; init; }
    // public DateTime Fecha { get; set; } = DateTime.Now;
    
    [Required(ErrorMessage = "El vendedor es obligatorio")]
    public int IdVendedor { get; set; }
    
    // Lista del detalle de la venta
    public List<DetalleVentaDto>? DetalleVenta { get; set; }
    
    [Required(ErrorMessage = "El total es obligatorio")]
    [Range(0.00, 99999999.99, ErrorMessage = "El precio debe estar entre 0.00 y 99,999,999.99")]
    
    public decimal Total { get; set; }
    
    // Listas para los dropDownList
    public IEnumerable<SelectListItem>? ListaUsuarios { get; set; }
    
    // Mensajes de notificación
    public string? Mensaje { get; set; }
    public string? TipoMensaje { get; set; }
    
}