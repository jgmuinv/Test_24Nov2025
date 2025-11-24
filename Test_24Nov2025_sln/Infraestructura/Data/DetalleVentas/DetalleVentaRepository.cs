using Contratos.General;
using Dominio.DetalleVentas;
using Microsoft.EntityFrameworkCore;
using Dominio.EncabezadoVentas;

namespace Infraestructura.Data.DetalleVentas;

public class DetalleVentaRepository : IDetalleVentaRepository
{
    private readonly ApplicationDbContext _context;

    public DetalleVentaRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // ==========================================
    // CRUD
    // ==========================================
    public async Task<DetalleVenta?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.DetalleVentas
            .AsNoTracking()  // Para consultas de solo lectura (mejor performance)
            .FirstOrDefaultAsync(p => p.Idde == id, ct);
    }

    public async Task<IReadOnlyList<DetalleVenta>> ListarAsync( int? idventa,CancellationToken ct = default)
    {
        var query = _context.DetalleVentas
            .Include(p=>p.Productos)
            .Include(ev=>ev.EncVenta)
            .AsNoTracking()
            .AsQueryable();
        
        // Filtro por id solo si viene con valor
        if (idventa.HasValue)
        {
            query = query.Where(p => p.Idventa == idventa.Value);
        }
        
        return await query
            .OrderBy(p => p.Fecha)
            .ToListAsync(ct);
    }

    public async Task CrearAsync(DetalleVenta obj, CancellationToken ct = default)
    {
        await _context.DetalleVentas.AddAsync(obj, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task ActualizarAsync(DetalleVenta obj, CancellationToken ct = default)
    {
        _context.DetalleVentas.Update(obj);
        await _context.SaveChangesAsync(ct);
    }

    public async Task EliminarAsync(int id, CancellationToken ct = default)
    {
        var obj = await _context.DetalleVentas.FindAsync(new object[] { id }, ct);
        
        if (obj != null)
        {
            _context.DetalleVentas.Remove(obj);
            await _context.SaveChangesAsync(ct);
        }
    }
    // ==========================================
    // Búsqueda paginada con filtros
    // ==========================================
    public async Task<PaginadoDto<DetalleVenta?>> ListarPaginadoAsync(
        int? idventa,
        int paginaActual,
        int registrosPorPagina,
        CancellationToken ct = default)
    {
        var query = _context.DetalleVentas            
            .Include(p=>p.Productos)
            .Include(ev =>ev.EncVenta)
            .AsNoTracking()
            .AsQueryable();

        // Aplicar filtros

        if (idventa.HasValue)
        {
            query = query.Where(p => p.Idventa == idventa.Value);
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
            

        return new PaginadoDto<DetalleVenta?>(items, total, paginaActual, registrosPorPagina);
    }
    
    // ==========================================
    // Métodos de Verificación
    // ==========================================
    public async Task<bool> ExisteAsync(int id, CancellationToken ct = default)
    {
        return await _context.DetalleVentas.AnyAsync(p => p.Idde == id, ct);
    }
    
    // ==========================================
    // Métodos con Includes (para cargar relaciones)
    // ==========================================
    public async Task<DetalleVenta?> ObtenerConProductosAsync(int id, CancellationToken ct = default)
    {
        return await _context.DetalleVentas
            .Include(p=>p.Productos)
            .Include(ev=>ev.EncVenta)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Idde == id, ct);
    }
}