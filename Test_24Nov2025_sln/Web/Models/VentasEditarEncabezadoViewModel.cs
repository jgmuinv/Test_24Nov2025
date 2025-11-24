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
    [Display(Name = "Código Prod.")]
    [Required (ErrorMessage = "Ingrese el código del nuevo detalle")]
    public int NuevoDvIdPro { get; set; }
    
    [Display(Name = "Nombre Prod.")]
    [Required (ErrorMessage = "Ingrese el nombre del nuevo detalle")]
    public string NuevoDvProducto { get; set; }
    
    [Display(Name = "Precio")]
    [Required (ErrorMessage = "Ingrese el precio del nuevo detalle")]
    public decimal NuevoDvPrecio { get; set; }
    
    [Display(Name = "Cantidad")]
    [Required (ErrorMessage = "Ingrese la cantidad del nuevo detalle")]
    public int NuevoDvCantidad { get; set; }
    
    // Mensajes de notificación
    public string? Mensaje { get; set; }
    public string? TipoMensaje { get; set; }
}