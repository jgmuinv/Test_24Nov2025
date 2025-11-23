using System.ComponentModel.DataAnnotations;
using Contratos.General;
using Contratos.Productos;

namespace Web.Models;

public class ProductosFiltroPaginadoViewModel
{
    // Filtros
    [Range(1, int.MaxValue)]
    [Display(Name = "Código")]
    public int? IdPro { get; set; }
    [StringLength(100)]
    public string? Nombre { get; set; }

    // Mensajes de notificación
    public string? Mensaje { get; set; }
    public string? TipoMensaje { get; set; }
    
    // Resultados
    public PaginadoDto<ProductoDto> Resultados { get; set; } =
        new (
            [],
            totalRegistros: 0,
            paginaActual: 1,
            tamanioPagina: 10);
}