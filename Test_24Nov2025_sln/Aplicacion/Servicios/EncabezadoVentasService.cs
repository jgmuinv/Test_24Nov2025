using Aplicacion.Interfaces;
using Contratos.EncabezadoVentas;
using Contratos.General;
using Dominio.Common;
using Dominio.EncabezadoVentas;

namespace Aplicacion.Servicios;

public class EncabezadoVentasService : IEncabezadoVentasService
{
    private readonly IEncabezadoVentaRepository _repo;

    // ==========================================
    // Listar con Filtros
    // ==========================================
    public async Task<ResultadoDto<IReadOnlyList<EncabezadoVentaDto?>>> ListarAsync(int? idvendedor,
        CancellationToken ct = default)
    {
        try
        {
            // Obtener todos los encabezados del repositorio
            var encabezadoVentas = await _repo.ListarAsync(idvendedor, ct);

            // Mapear a DTOs
            var encabezadoVentaDto = encabezadoVentas
                .Select(p => MapearADto(p))
                .ToList();

            return ResultadoDto<IReadOnlyList<EncabezadoVentaDto?>>.Success(encabezadoVentaDto.AsReadOnly());
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al listar los encabezados de venta", ex);
        }
    }

    // ==========================================
    // Listar Paginado con Filtros
    // ==========================================
    public async Task<ResultadoDto<PaginadoDto<EncabezadoVentaDto?>>> ListarPaginadoAsync(int? idvendedor,
        int paginaActual, int registrosPorPagina,
        CancellationToken ct = default)
    {
        try
        {
            // Aplica validaciones de negocio
            if (paginaActual <= 0) throw new DomainException("La página actual debe ser mayor a cero");

            if (registrosPorPagina <= 0)
                throw new DomainException("La la cantidad de registros por página debe ser mayor a cero");

            // Obtener todos los encabezados de venta del repositorio
            var encabezadoVentas = await _repo.ListarPaginadoAsync(idvendedor, paginaActual, registrosPorPagina, ct);

            // Mapear a DTOs
            var encabezadosVentasDto = encabezadoVentas.Items
                .Select(p => MapearADto(p))
                .ToList();
            var paginado = new PaginadoDto<EncabezadoVentaDto?>(encabezadosVentasDto, encabezadoVentas.TotalRegistros,
                encabezadoVentas.PaginaActual,
                encabezadoVentas.TamanioPagina);

            return ResultadoDto<PaginadoDto<EncabezadoVentaDto?>>.Success(paginado);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al listar los encabezados de venta", ex);
        }
    }

    // ==========================================
    // Crear registro
    // ==========================================
    public async Task<ResultadoDto<EncabezadoVentaDto?>> CrearAsync(CrearEncabezadoVentaDto dto,
        CancellationToken ct = default)
    {
        try
        {
            var encabezadoVenta = new EncabezadoVenta(dto.Idvendedor, dto.Total);
            await _repo.CrearAsync(encabezadoVenta, ct);

            return ResultadoDto<EncabezadoVentaDto?>.Success(MapearADto(encabezadoVenta));
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
    public async Task<ResultadoDto<EncabezadoVentaDto?>> EditarAsync(int id, EditarEncabezadoVentaDto dto,
        CancellationToken ct = default)
    {
        try
        {
            // Obtener el registro existente
            var objBd = await _repo.ObtenerPorIdAsync(id, ct);

            if (objBd == null)
            {
                return ResultadoDto<EncabezadoVentaDto?>.Failure("No existe el registro en la base de datos");
            }

            if (dto.Total != objBd.DetalleVenta.Sum(venta => venta.Total))
            {
                return ResultadoDto<EncabezadoVentaDto?>.Failure("El total no coincide con el detalle de la venta");
            }
            
            var resultadoPrecio = objBd.ActualizarTotal(dto.Total);
            if (!resultadoPrecio.Exitoso)
            {
                return resultadoPrecio;
            }

            await _repo.ActualizarAsync(objBd, ct);

            return ResultadoDto<EncabezadoVentaDto?>.Success(MapearADto(objBd));
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

            // Verificar si tiene ventas asociadas
            var encabezadoConVentas = await _repo.ObtenerConDetallesVentaAsync(id, ct);

            if (encabezadoConVentas?.DetalleVenta?.Any() == true)
            {
                throw new DomainException(
                    "No se puede eliminar el registro porque tiene ventas asociadas");
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
    private static EncabezadoVentaDto MapearADto(EncabezadoVenta producto)
    {
        return new EncabezadoVentaDto()
        {
            Idventa = producto.Idventa,
            Idvendedor = producto.Idvendedor,
            Fecha = producto.Fecha,
            Total = producto.Total
        };
    }
}