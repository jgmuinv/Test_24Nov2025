using System.ComponentModel.DataAnnotations;
using Contratos.EncabezadoVentas;
using Contratos.Productos;

namespace Contratos.DetalleVentas;
/// <summary>
/// DTO para representar un detalle de venta
/// </summary>
public record DetalleVentaDto
{
    public int Idde { get; set; }

    public DateTime Fecha { get; set; }

    public int Idventa { get; set; }
    //public EncabezadoVentaDto? EncabezadoVenta { get; set; }

    public int Idpro { get; set; }

    public string NombreProducto { get; set; }
    //public ProductoDto? Producto { get; set; }

    public decimal Cantidad { get; set; }

    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Precio { get; set; }

    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Iva { get; set; }

    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Total { get; set; }
}