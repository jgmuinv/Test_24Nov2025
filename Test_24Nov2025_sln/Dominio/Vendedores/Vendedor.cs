using Dominio.EncabezadoVentas;
using System;
using System.Collections.Generic;

namespace Dominio.Vendedores;

public partial class Vendedor
{
    public int Idvendedor { get; set; }

    public string Nombre { get; set; } = null!;

    //public virtual ICollection<EncabezadoVenta> EncabezadoVenta { get; set; } = new List<EncabezadoVenta>();
}
