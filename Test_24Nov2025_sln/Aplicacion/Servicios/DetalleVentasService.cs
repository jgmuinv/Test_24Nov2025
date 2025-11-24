using Aplicacion.Interfaces;
using Contratos.DetalleVentas;
using Contratos.General;
using Dominio.Common;
using Dominio.DetalleVentas;

namespace Aplicacion.Servicios;

public class DetalleVentasService : IDetalleVentasService
{
    private readonly IDetalleVentaRepository _repo;

    public DetalleVentasService(IDetalleVentaRepository repo)
    {
        _repo = repo;
    }
    
    // ==========================================
    // Listar con Filtros
    // ==========================================
    public async Task<ResultadoDto<IReadOnlyList<DetalleVentaDto?>>> ListarAsync(int? idvendedor,
        CancellationToken ct = default)
    {
        try
        {
            // Obtener todos los Detalles del repositorio
            var DetalleVentas = await _repo.ListarAsync(idvendedor, ct);

            // Mapear a DTOs
            var DetalleVentaDto = DetalleVentas
                .Select(p => MapearADto(p))
                .ToList();

            return ResultadoDto<IReadOnlyList<DetalleVentaDto?>>.Success(DetalleVentaDto.AsReadOnly());
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al listar los Detalles de venta", ex);
        }
    }

    // ==========================================
    // Listar Paginado con Filtros
    // ==========================================
    public async Task<ResultadoDto<PaginadoDto<DetalleVentaDto?>>> ListarPaginadoAsync(int? idventa,
        int paginaActual, int registrosPorPagina,
        CancellationToken ct = default)
    {
        try
        {
            // Aplica validaciones de negocio
            if (paginaActual <= 0) throw new DomainException("La página actual debe ser mayor a cero");

            if (registrosPorPagina <= 0)
                throw new DomainException("La la cantidad de registros por página debe ser mayor a cero");

            // Obtener todos los Detalles de venta del repositorio
            var DetalleVentas = await _repo.ListarPaginadoAsync(idventa, paginaActual, registrosPorPagina, ct);

            // Mapear a DTOs
            var DetallesVentasDto = DetalleVentas.Items
                .Select(p => MapearADto(p))
                .ToList();
            var paginado = new PaginadoDto<DetalleVentaDto?>(DetallesVentasDto, DetalleVentas.TotalRegistros,
                DetalleVentas.PaginaActual,
                DetalleVentas.TamanioPagina);

            return ResultadoDto<PaginadoDto<DetalleVentaDto?>>.Success(paginado);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al listar los Detalles de venta", ex);
        }
    }

    // ==========================================
    // Crear registro
    // ==========================================
    public async Task<ResultadoDto<DetalleVentaDto?>> CrearAsync(CrearDetalleVentaDto dto,
        CancellationToken ct = default)
    {
        try
        {
            var detalleVenta = new DetalleVenta(dto.Idventa, dto.Idpro, dto.Cantidad, dto.Precio);
            await _repo.CrearAsync(detalleVenta, ct);

            return ResultadoDto<DetalleVentaDto?>.Success(MapearADto(detalleVenta));
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al crear el registro", ex);
        }
    }

    // ==========================================
    // Editar registro
    // ==========================================
    public async Task<ResultadoDto<DetalleVentaDto?>> EditarAsync(int id, EditarDetalleVentaDto dto,
        CancellationToken ct = default)
    {
        try
        {
            // Obtener el registro existente
            var objBd = await _repo.ObtenerPorIdAsync(id, ct);

            if (objBd == null)
            {
                return ResultadoDto<DetalleVentaDto?>.Failure("No existe el registro en la base de datos");
            }

            if (dto.Idpro <= 0)
            {
                return ResultadoDto<DetalleVentaDto?>.Failure("Indique el producto");
            }
            
            if (objBd.Idpro != dto.Idpro)
            {
                var resultadoProducto = objBd.ActualizarProducto(dto.Idpro);
                if (!resultadoProducto.Exitoso)
                {
                    return resultadoProducto;
                }                
            }
            
            if (dto.Cantidad <= 0)
            {
                return ResultadoDto<DetalleVentaDto?>.Failure("La cantidad debe ser positiva");
            }
            
            if (dto.Iva <= 0)
            {
                return ResultadoDto<DetalleVentaDto?>.Failure("El IVA debe ser positivo");
            }
            
            if (objBd.Precio != dto.Precio)
            {
                var resultadoPrecio = objBd.ActualizarPrecio(dto.Precio);
                if (!resultadoPrecio.Exitoso)
                {
                    return resultadoPrecio;
                }                
            }
            
            if (dto.Total != dto.Cantidad * dto.Precio)
            {
                return ResultadoDto<DetalleVentaDto?>.Failure("El total no coincide con el detalle de la venta");
            }

            if (objBd.Total != dto.Total)
            {
                var resultadoTotal = objBd.ActualizarTotal(dto.Total);
                if (!resultadoTotal.Exitoso)
                {
                    return resultadoTotal;
                }                
            }
            
            await _repo.ActualizarAsync(objBd, ct);

            return ResultadoDto<DetalleVentaDto?>.Success(MapearADto(objBd));
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error al editar el registro con ID {id}", ex);
        }
    }

    // ==========================================
    // Eliminar registro
    // ==========================================
    public async Task<ResultadoDto<bool?>> EliminarAsync(int id, CancellationToken ct = default)
    {
        try
        {
            // Verificar que existe
            if (!await _repo.ExisteAsync(id, ct))
            {
                throw new DomainException("No existe un registro con ese ID");
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
    // Métodos Auxiliares Privados
    // ==========================================

    /// <summary>
    /// Mapea una entidad Producto a ProductoDto
    /// </summary>
    private static DetalleVentaDto MapearADto(DetalleVenta producto)
    {
        return new DetalleVentaDto()
        {
            Idde = producto.Idde,
            Idventa = producto.Idventa,
            Idpro = producto.Idpro,
            Cantidad = producto.Cantidad,
            Precio = producto.Precio,
            Total = producto.Total,
            Fecha = producto.Fecha,
            Iva = producto.Iva
        };
    }
}