using Aplicacion.Interfaces;
using Contratos.General;
using Dominio.Productos;
using Dominio.Common;
using Contratos.Productos;

namespace Aplicacion.Servicios;

/// <summary>
/// Servicio de aplicación para gestionar la lógica de negocio de productos
/// </summary>
public class ProductosService : IProductosService
{
    private readonly IProductoRepository _repo;
    
    public ProductosService(IProductoRepository repo)
    {
        _repo = repo;
    }

    // ==========================================
    // Listar con Filtros
    // ==========================================
    public async Task<ResultadoDto<IReadOnlyList<ProductoDto?>>> ListarAsync(int? idpro, string? nombre, CancellationToken ct = default)
    {
        try
        {
            // Obtener todos los productos del repositorio
            var productos = await _repo.ListarAsync(idpro, nombre, ct);

            // Mapear a DTOs
            var productosDto = productos
                .Select(p => MapearADto(p))
                .ToList();

            return ResultadoDto<IReadOnlyList<ProductoDto?>>.Success(productosDto.AsReadOnly());
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al listar los productos", ex);
        }
    }
    
    // ==========================================
    // Listar Paginado con Filtros
    // ==========================================
    public async Task<ResultadoDto<PaginadoDto<ProductoDto?>>> ListarPaginadoAsync(int? idpro, string? nombre, int paginaActual, int registrosPorPagina, CancellationToken ct = default)
    {
        try
        {
            // Aplica validaciones de negocio
            if (paginaActual <= 0) throw new DomainException("La página actual debe ser mayor a cero");
            
            if (registrosPorPagina <= 0) throw new DomainException("La la cantidad de registros por página debe ser mayor a cero");
            
            // Obtener todos los productos del repositorio
            var productos = await _repo.ListarPaginadoAsync(idpro, nombre, paginaActual, registrosPorPagina, ct);

            // Mapear a DTOs
            var productosDto = productos.Items
                .Select(p => MapearADto(p))
                .ToList();
            var paginado = new PaginadoDto<ProductoDto?>(productosDto, productos.TotalRegistros, productos.PaginaActual,
                productos.TamanioPagina);
                
            return ResultadoDto<PaginadoDto<ProductoDto?>>.Success(paginado);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al listar los productos", ex);
        }
    }

    // ==========================================
    // Crear registro
    // ==========================================
    public async Task<ResultadoDto<ProductoDto?>> CrearAsync(CrearProductoDto dto, CancellationToken ct = default)
    {
        try
        {        
            // 1. Validar que no exista un producto con el mismo nombre
            if (await _repo.ExisteNombreAsync(dto.Nombre, ct))
            {
                throw new DomainException("Ya existe un producto con ese nombre");
                //return ResultadoDto<ProductoDto?>.Failure("Ya existe un producto con ese nombre");
            }
            
            var producto = new Producto(dto.Nombre, dto.Precio);
            await _repo.CrearAsync(producto, ct);
            
            return ResultadoDto<ProductoDto?>.Success(MapearADto(producto));
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al crear el producto", ex);
        }
    }

    // ==========================================
    // Editar Producto
    // ==========================================
    public async Task<ResultadoDto<ProductoDto?>> EditarAsync(
        int id, 
        EditarProductoDto dto, 
        CancellationToken ct = default)
    {
        try
        {        
            // Obtener el producto existente
            var producto = await _repo.ObtenerPorIdAsync(id, ct);

            if (producto == null)
            {
                return ResultadoDto<ProductoDto?>.Failure("No existe un producto con ese código");
            }

            // Validar que no exista otro producto con el mismo nombre
            if (await _repo.ExisteNombreAsync(dto.Nombre, id, ct))
            {
                return ResultadoDto<ProductoDto?>.Failure("Ya existe otro producto con ese nombre");
            }
            
            producto.ActualizarNombre(dto.Nombre);

            if (dto.Precio.HasValue)
            {
                var resultadoPrecio = producto.ActualizarPrecio(dto.Precio.Value);
                if (!resultadoPrecio.Exitoso)
                {
                    
                    return resultadoPrecio;
                }
            }
            
            await _repo.ActualizarAsync(producto, ct);
            
            return ResultadoDto<ProductoDto?>.Success(MapearADto(producto));
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error al editar el producto con ID {id}", ex);
        }
    }

    // ==========================================
    // Eliminar Producto
    // ==========================================
    public async Task<ResultadoDto<bool?>> EliminarAsync(int id, CancellationToken ct = default)
    {
        try
        {        
            // Verificar que existe
            if (!await _repo.ExisteAsync(id, ct))
            {
                throw new DomainException("No existe un producto con ese ID") ;
            }

            // Verificar si tiene ventas asociadas
            var productoConVentas = await _repo.ObtenerConDetallesVentaAsync(id, ct);
            
            if (productoConVentas?.DetalleVenta?.Any() == true)
            {
                throw new DomainException(
                    "No se puede eliminar el producto porque tiene ventas asociadas");
            }

            // Eliminar
            await _repo.EliminarAsync(id, ct);

            return ResultadoDto<bool?>.Success(true);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error al eliminar el producto con ID {id}", ex);
        }
    }

    // ==========================================
    // Métodos Auxiliares
    // ==========================================

    /// <summary>
    /// Mapea una entidad Producto a ProductoDto
    /// </summary>
    public static ProductoDto MapearADto(Producto producto)
    {
        return new ProductoDto
        {
            Id = producto.idpro,
            Nombre = producto.producto,
            Precio = producto.precio ?? 0
        };
    }
}