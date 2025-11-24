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

    public decimal Precio { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }
}