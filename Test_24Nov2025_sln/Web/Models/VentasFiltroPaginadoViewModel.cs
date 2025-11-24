using System.ComponentModel.DataAnnotations;
using Contratos.EncabezadoVentas;
using Contratos.General;
using Contratos.Productos;
using Contratos.Usuarios;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Models;

public class VentasFiltroPaginadoViewModel
{
    // Filtros
    [Range(1, int.MaxValue)]
    [Display(Name = "Vendedor")]
    public int? IdVendedor { get; set; }
    
    // Listas para los select
    public IEnumerable<SelectListItem>? ListaUsuarios { get; set; }
    
    // Mensajes de notificación
    public string? Mensaje { get; set; }
    public string? TipoMensaje { get; set; }
    
    // Resultados
    public PaginadoDto<EncabezadoVentaDto> Resultados { get; set; } =
        new (
            [],
            totalRegistros: 0,
            paginaActual: 1,
            tamanioPagina: 10);
}