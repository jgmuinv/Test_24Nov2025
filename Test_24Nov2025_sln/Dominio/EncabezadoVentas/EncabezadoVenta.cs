using Dominio.DetalleVentas;
using Dominio.Vendedores;
using Dominio.Common;
using Contratos.General;
using System;
using System.Collections.Generic;
using Contratos.EncabezadoVentas;

namespace Dominio.EncabezadoVentas;

public partial class EncabezadoVenta
{
    public int Idventa { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    public int Idvendedor { get; set; }

    public decimal Total { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual Vendedor IdvendedorNavigation { get; set; } = null!;
    
    // Constructor privado para EF Core
    private EncabezadoVenta() { }
    
    // Constructor público para crear instancias (sin ID para nuevos registros)
    public EncabezadoVenta(int idvendedor_, decimal total_)
    {
        // Validaciones de negocio
        if (idvendedor_ <= 0)
            throw new DomainException("El id del vendedor es obligatorio");

        if (total_ <= 0 || total_ > (decimal) 99999999.99)
            throw new DomainException("El precio debe ser positivo y menor a 99,999,999.99");

        // Asignación de valores a las propiedades si pasa validaciones
        Fecha = DateTime.Now;
        Idvendedor = idvendedor_;
        Total = total_;
    }
    
    // Constructor sobrecargado para cuando ya existe ID (para testing o casos especiales)
    public EncabezadoVenta(int idventa_, int idvendedor_, decimal total_) : this(idvendedor_, total_)
    {
        Idventa = idventa_;
    }
    
    // Métodos que encapsulan reglas de negocio más complejas
    public ResultadoDto<EncabezadoVentaDto?> ActualizarTotal(decimal nuevoTotal)
    {
        if (nuevoTotal <= 0 || nuevoTotal > (decimal) 99999999.99)
            return ResultadoDto<EncabezadoVentaDto?>.Failure("El precio debe ser positivo y menor a 99,999,999.99");
        
        Total = nuevoTotal;
        return ResultadoDto<EncabezadoVentaDto?>.Success(null);
    }
}
