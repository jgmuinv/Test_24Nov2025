using System.ComponentModel.DataAnnotations;
using Contratos.DetalleVentas;
using Contratos.EncabezadoVentas;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Models;

public record VentasEditarEncabezadoViewModel : EncabezadoVentaDto
{
    // // Listas para los dropDownList
    // public IEnumerable<SelectListItem>? ListaUsuarios { get; set; }
    
    // Nuevo detalle de venta
    public int? NuevoDvIdPro { get; set; }
    public int? NuevoDvProducto { get; set; }
    public decimal? NuevoDvPrecio { get; set; }
    public int? NuevoDvCantidad { get; set; }
    
    // Mensajes de notificación
    public string? Mensaje { get; set; }
    public string? TipoMensaje { get; set; }
}