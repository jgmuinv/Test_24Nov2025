using System.ComponentModel.DataAnnotations;

namespace Contratos.EncabezadoVentas;

/// <summary>
/// DTO para representar un encabezado de venta
/// </summary>
public record EncabezadoVentaDto
{
    public int Idventa { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    public int Idvendedor { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Total { get; set; }
}