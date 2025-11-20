using System;
using System.Collections.Generic;
using Dominio.Common;
using Dominio;
using Dominio.Productos;
using Dominio.EncabezadoVentas;

namespace Dominio.DetalleVentas;

public partial class DetalleVenta
{
    public int Idde { get; set; }

    public DateTime Fecha { get; set; }

    public int Idventa { get; set; }

    public int Idpro { get; set; }

    public decimal Cantidad { get; set; }

    public decimal Precio { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }

    public virtual Producto IdproNavigation { get; set; } = null!;

    public virtual EncabezadoVenta IdventaNavigation { get; set; } = null!;
}
