using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Common;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dominio.Productos
{
    public class Productos
    {
        // Propiedades
        public int idpro { get; set; }
        public string producto { get; set; }
        public decimal precio { get; set; }

        // Constructor privado para EF Core
        private Productos() { }

        // Constructor público para crear instancias
        public Productos(int idpro_, string producto_, decimal precio_)
        {
            // Validaciones de negocio

            if (string.IsNullOrWhiteSpace(producto))
                throw new DomainException("El nombre es obligatorio");

            if (precio <= 0)
                throw new DomainException("El precio debe ser positivo");

            // Asignación de valores a las propiedades si pasa validaciones
            this.idpro = idpro_;
            this.producto = producto_;
            this.precio = precio_;
        }


        // Métodos que encapsulan reglas de negocio más complejas
        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
                throw new DomainException("El precio debe ser positivo");

            // Regla de negocio específica
            if (nuevoPrecio < precio * 0.5m)
                throw new DomainException("No se puede reducir el precio más del 50% de una sola vez.");

            precio = nuevoPrecio;
        }
    }
}
