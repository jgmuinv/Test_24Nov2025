using System;
using System.Collections.Generic;
using Dominio.EncabezadoVentas;

namespace Dominio.Usuarios;

public partial class Usuario
{
    public int idus { get; set; }

    public string usuario { get; set; } = null!;

    public string nombre { get; set; } = null!;

    public byte[] clavehash { get; set; } = null!;

    public byte[]? clavesalt { get; set; }

    public string? clavealgoritmo { get; set; }

    public int? claveiteraciones { get; set; }

    public virtual ICollection<EncabezadoVenta> EncabezadoVentas { get; set; } = new List<EncabezadoVenta>();
}
