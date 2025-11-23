using Contratos.General;
using Microsoft.EntityFrameworkCore;
using Dominio.EncabezadoVentas;

namespace Infraestructura.Data.EncabezadoVentas;

public class EncabezadoVentaRepository : IEncabezadoVentaRepository
{
    private readonly ApplicationDbContext _context;

    public EncabezadoVentaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // CRUD
    // ==========================================
    public async Task<EncabezadoVenta?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.EncabezadoVentas
            .AsNoTracking()  // Para consultas de solo lectura (mejor performance)
            .FirstOrDefaultAsync(p => p.Idventa == id, ct);
    }

    public async Task<IReadOnlyList<EncabezadoVenta>> ListarAsync( int? idvendedor,CancellationToken ct = default)
    {
        var query = _context.EncabezadoVentas
            .Include(dv=>dv.DetalleVenta)
            .AsNoTracking()
            .AsQueryable();
        
        // Filtro por id solo si viene con valor
        if (idvendedor.HasValue)
        {
            query = query.Where(p => p.Idvendedor == idvendedor.Value);
        }
        
        return await query
            .OrderBy(p => p.Fecha)
            .ToListAsync(ct);
    }

    public async Task CrearAsync(EncabezadoVenta obj, CancellationToken ct = default)
    {
        await _context.EncabezadoVentas.AddAsync(obj, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task ActualizarAsync(EncabezadoVenta EncabezadoVenta, CancellationToken ct = default)
    {
        _context.EncabezadoVentas.Update(EncabezadoVenta);
        await _context.SaveChangesAsync(ct);
    }

    public async Task EliminarAsync(int id, CancellationToken ct = default)
    {
        var EncabezadoVenta = await _context.EncabezadoVentas.FindAsync(new object[] { id }, ct);
        
        if (EncabezadoVenta != null)
        {
            _context.EncabezadoVentas.Remove(EncabezadoVenta);
            await _context.SaveChangesAsync(ct);
        }
    }
    // ==========================================
    // Búsqueda paginada con filtros
    // ==========================================
    public async Task<PaginadoDto<EncabezadoVenta?>> ListarPaginadoAsync(
        int? idvendedor,
        int paginaActual,
        int registrosPorPagina,
        CancellationToken ct = default)
    {
        var query = _context.EncabezadoVentas            
            .Include(dv=>dv.DetalleVenta)
            .AsNoTracking()
            .AsQueryable();

        // Aplicar filtros

        if (idvendedor.HasValue)
        {
            query = query.Where(p => p.Idvendedor == idvendedor.Value);
        }

        // Contar total (Antes de ordenar y paginar)
        var total = await query.CountAsync(ct);
        
        var paginas = (int)Math.Ceiling(total / (double)registrosPorPagina);

        // Aplicar paginación
        var items = await query
            .OrderBy(p => p.Idventa)
            .Skip(registrosPorPagina * (paginaActual - 1))
            .Take(registrosPorPagina)
            .ToListAsync(ct);
            

        return new PaginadoDto<EncabezadoVenta?>(items, total, paginaActual, registrosPorPagina);
    }
    
    // ==========================================
    // Métodos de Verificación
    // ==========================================
    public async Task<bool> ExisteAsync(int id, CancellationToken ct = default)
    {
        return await _context.EncabezadoVentas.AnyAsync(p => p.Idventa == id, ct);
    }
    
    // ==========================================
    // Métodos con Includes (para cargar relaciones)
    // ==========================================
    public async Task<EncabezadoVenta?> ObtenerConDetallesVentaAsync(int id, CancellationToken ct = default)
    {
        return await _context.EncabezadoVentas
            .Include(dv => dv.DetalleVenta)
            .ThenInclude(dv => dv.Productos)
            //.ThenInclude(dv => dv.EncVenta)
            //.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Idventa == id, ct);
    }
}