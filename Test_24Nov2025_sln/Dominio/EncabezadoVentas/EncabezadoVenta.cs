using Dominio.DetalleVentas;
using Dominio.Vendedores;
using Dominio.Common;
using Contratos.General;
using System;
using System.Collections.Generic;
using Contratos.EncabezadoVentas;
using Dominio.Usuarios;

namespace Dominio.EncabezadoVentas;

public sealed class EncabezadoVenta
{
    public int Idventa { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    public int Idvendedor { get; set; }

    public decimal Total { get; set; }

    public ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public Usuario Usuario { get; set; } = null!;
    
    // Constructor privado para EF Core
    private EncabezadoVenta() { }
    
    // Constructor público para crear instancias (sin ID para nuevos registros)
    public EncabezadoVenta(int idvendedor_)
    {
        // Validaciones de negocio
        if (idvendedor_ <= 0)
            throw new DomainException("El id del vendedor es obligatorio");
        
        // Asignación de valores a las propiedades si pasa validaciones
        Fecha = DateTime.Now;
        Idvendedor = idvendedor_;
        ActualizarTotal();
    }
    
    // Constructor sobrecargado para cuando ya existe ID (para testing o casos especiales)
    public EncabezadoVenta(int idventa_, int idvendedor_) : this(idvendedor_)
    {
        Idventa = idventa_;
        ActualizarTotal();
    }
    
    // Métodos que encapsulan reglas de negocio más complejas
    public ResultadoDto<EncabezadoVentaDto?> ActualizarTotal()
    {
        var nuevoTotal = DetalleVenta.Sum(x => x.Total);
        if (nuevoTotal <= 0 || nuevoTotal > 99999999.99m)
            return ResultadoDto<EncabezadoVentaDto?>.Failure("El precio debe ser positivo y menor a 99,999,999.99");
        
        Total = nuevoTotal;
        return ResultadoDto<EncabezadoVentaDto?>.Success(null);
    }
    
    public ResultadoDto<EncabezadoVentaDto?> ActualizarVendedor(int nuevoVendedor)
    {
        if (nuevoVendedor <= 0)
            return ResultadoDto<EncabezadoVentaDto?>.Failure("Debe seleccionar un vendedor");
        
        Idvendedor = nuevoVendedor;
        return ResultadoDto<EncabezadoVentaDto?>.Success(null);
    }
}
