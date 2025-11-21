using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contratos.General;
using Contratos.Productos;
using Dominio.Common;
using Dominio.DetalleVentas;

//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dominio.Productos
{
    public partial class Producto
    {
        public int idpro { get; set; }

        public string producto { get; set; } = null!;

        public decimal? precio { get; set; }

        public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

        // Constructor privado para EF Core
        private Producto() { }

        // Constructor público para crear instancias (sin ID para nuevos registros)
        public Producto(string producto_, decimal precio_)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(producto_))
                throw new DomainException("El nombre es obligatorio");

            if (precio_ <= 0)
                throw new DomainException("El precio debe ser positivo");

            // Asignación de valores a las propiedades si pasa validaciones
            this.producto = producto_;
            this.precio = precio_;
        }

        // Constructor sobrecargado para cuando ya existe ID (para testing o casos especiales)
        public Producto(int idpro_, string producto_, decimal precio_) : this(producto_, precio_)
        {
            this.idpro = idpro_;
        }

        // Métodos que encapsulan reglas de negocio más complejas
        public ResultadoDto<ProductoDto?> ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
                return ResultadoDto<ProductoDto?>.Failure("El precio debe ser positivo");

            // Regla de negocio específica
            if (precio.HasValue && nuevoPrecio < precio * 0.5m)
                return ResultadoDto<ProductoDto?>.Failure("No se puede reducir el precio más del 50% de una sola vez.");

            precio = nuevoPrecio;
            return ResultadoDto<ProductoDto?>.Success(null);
        }
        
        public void ActualizarNombre(string nuevoNombre)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new DomainException("El nombre es obligatorio");

            if (nuevoNombre.Length > 100)
                throw new DomainException("El nombre no puede exceder 100 caracteres");

            producto = nuevoNombre;
        }
    }

}
