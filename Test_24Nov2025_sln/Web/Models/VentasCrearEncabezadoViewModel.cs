using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Models;

public class VentasCrearEncabezadoViewModel
{
    // Vendedor seleccionado
    [Range(1, int.MaxValue)]
    [Display(Name = "Vendedor")]
    [Required(ErrorMessage = "El vendedor es obligatorio")]
    public int IdVendedor { get; set; }
    
    // Listas para los dropDownList
    public IEnumerable<SelectListItem>? ListaUsuarios { get; set; }
    
    // Mensajes de notificación
    public string? Mensaje { get; set; }
    public string? TipoMensaje { get; set; }
}