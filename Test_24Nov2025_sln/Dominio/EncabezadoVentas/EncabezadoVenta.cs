using Dominio.DetalleVentas;
using Dominio.Vendedores;
using System;
using System.Collections.Generic;

namespace Dominio.EncabezadoVentas;

public partial class EncabezadoVenta
{
    public int Idventa { get; set; }

    public DateTime Fecha { get; set; }

    public int Idvendedor { get; set; }

    public decimal Total { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual Vendedor IdvendedorNavigation { get; set; } = null!;
}
