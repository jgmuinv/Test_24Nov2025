using System.ComponentModel.DataAnnotations;
using Contratos.DetalleVentas;

namespace Contratos.EncabezadoVentas;

/// <summary>
/// DTO para representar un encabezado de venta
/// </summary>
public record EncabezadoVentaDto
{
    [Required (ErrorMessage = "El Id del encabezado es obligatorio")]
    public int Idventa { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [Range(1, int.MaxValue)]
    [Display(Name = "Vendedor")]
    [Required(ErrorMessage = "El vendedor es obligatorio")]
    public int Idvendedor { get; set; }
    public string NombreVendedor { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Total { get; set; }
    //public ICollection<DetalleVentaDto> DetalleVenta { get; set; } = new List<DetalleVentaDto>();
    public List<DetalleVentaDto>? DetalleVenta { get; set; }
}