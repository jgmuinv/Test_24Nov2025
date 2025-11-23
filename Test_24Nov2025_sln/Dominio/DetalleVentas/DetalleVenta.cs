using System;
using System.Collections.Generic;
using Contratos.General;
using Contratos.Productos;
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

    public virtual Producto Productos { get; set; } = null!;

    public virtual EncabezadoVenta EncVenta { get; set; } = null!;
    
    // Constructor privado para EF Core
    private DetalleVenta() { }
    
    // Constructor público para crear instancias (sin ID para nuevos registros)
        public DetalleVenta(int idVenta, int idProducto, int cantidad, decimal precio)
        {
            // Validaciones de negocio
            if (idProducto <= 0)
                throw new DomainException("Debe seleccionar un producto");
            
            if (cantidad <= 0)
                throw new DomainException("La cantidad debe ser mayor a cero");
            
            if (precio <= 0 || precio > 99999999.99m)
                throw new DomainException("El precio debe ser positivo y menor a 99,999,999.99");
            
            // Asignación de valores a las propiedades si pasa validaciones
            Fecha = DateTime.Now;
            Iva = (cantidad * precio) * 0.13m;
            Total= (cantidad * precio) + Iva;
            Idventa = idVenta;
            Idpro = idProducto;
            Cantidad = cantidad;
            Precio = precio;
        }

        // Constructor sobrecargado para cuando ya existe ID (para testing o casos especiales)
        public DetalleVenta(int idDe, int idVenta, int idProducto, int cantidad, decimal precio) : this(idVenta, idProducto, cantidad, precio)
        {
            this.Idde = idDe;
        }

        // Métodos que encapsulan reglas de negocio más complejas
        public ResultadoDto<ProductoDto?> ActualizarPrecio(decimal nuevoPrecio)
        {
            // Regla de negocio específica
            if (nuevoPrecio <= 0 || nuevoPrecio > 99999999.99m)
                return ResultadoDto<ProductoDto?>.Failure("El precio debe ser positivo y menor a 99,999,999.99");
            
            Precio = nuevoPrecio;
            return ResultadoDto<ProductoDto?>.Success(null);
        }
}
