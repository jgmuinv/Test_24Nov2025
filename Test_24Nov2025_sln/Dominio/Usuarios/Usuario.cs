using System;
using System.Collections.Generic;

namespace Dominio.Usuarios;

public partial class Usuario
{
    public int Idus { get; set; }

    public string Usuario1 { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public byte[] Clavehash { get; set; } = null!;

    public byte[]? Clavesalt { get; set; }

    public string? Clavealgoritmo { get; set; }

    public int? Claveiteraciones { get; set; }
}
