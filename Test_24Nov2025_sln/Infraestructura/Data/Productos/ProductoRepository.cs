using Contratos.General;
using Dominio.Productos;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infraestructura.Data.Productos;

public class ProductoRepository : IProductoRepository
{
    private readonly ApplicationDbContext _context;

    public ProductoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // Métodos de la Interfaz
    // ==========================================

    public async Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Productos
            .AsNoTracking()  // Para consultas de solo lectura (mejor performance)
            .FirstOrDefaultAsync(p => p.idpro == id, ct);
    }

    public async Task<IReadOnlyList<Producto>> ListarAsync( int? idpro, string? nombre,CancellationToken ct = default)
    {
        var query = _context.Productos
            .AsNoTracking()
            .AsQueryable();
        
        // Filtro por id solo si viene con valor
        if (idpro.HasValue)
        {
            query = query.Where(p => p.idpro == idpro.Value);
        }

        // Filtro por nombre solo si viene con texto
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(p => p.producto.Contains(nombre));
        }
        
        return await query
            .OrderBy(p => p.producto)
            .ToListAsync(ct);
    }

    public async Task CrearAsync(Producto producto, CancellationToken ct = default)
    {
        await _context.Productos.AddAsync(producto, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task ActualizarAsync(Producto producto, CancellationToken ct = default)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync(ct);
    }

    public async Task EliminarAsync(int id, CancellationToken ct = default)
    {
        var producto = await _context.Productos.FindAsync(new object[] { id }, ct);
        
        if (producto != null)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync(ct);
        }
    }

    // ==========================================
    // Métodos Adicionales para Paginación y Búsqueda
    // (usados por ProductosService)
    // ==========================================

    public async Task<int> ContarTodos(CancellationToken ct = default)
    {
        return await _context.Productos.CountAsync(ct);
    }

    public async Task<List<Producto>> ObtenerPaginado(int saltar, int tomar, CancellationToken ct = default)
    {
        return await _context.Productos
            .AsNoTracking()
            .OrderBy(p => p.producto)  // Importante: siempre ordenar antes de paginar
            .Skip(saltar)
            .Take(tomar)
            .ToListAsync(ct);
    }

    // ==========================================
    // Búsqueda con filtros
    // ==========================================

    public async Task<PaginadoDto<Producto>> BuscarPaginadoAsync(int? idpro,
        string? nombre,
        int paginaActual,
        int registrosPorPagina,
        CancellationToken ct = default)
    {
        var query = _context.Productos.AsNoTracking();

        // Aplicar filtros

        if (idpro.HasValue)
        {
            query = query.Where(p => p.idpro == idpro.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(p => p.producto.Contains(nombre));
        }

        // Contar total (Antes de ordenar y paginar)
        var total = await query.CountAsync(ct);
        
        var paginas = (int)Math.Ceiling(total / (double)registrosPorPagina);

        // Aplicar paginación
        var items = await query
            .OrderBy(p => p.producto)
            .Skip(registrosPorPagina * (paginaActual - 1))
            .Take(registrosPorPagina)
            .ToListAsync(ct);

        return new PaginadoDto<Producto>(items, total, paginaActual, registrosPorPagina);
    }

    // ==========================================
    // Métodos de Verificación
    // ==========================================

    public async Task<bool> ExisteAsync(int id, CancellationToken ct = default)
    {
        return await _context.Productos.AnyAsync(p => p.idpro == id, ct);
    }

    public async Task<bool> ExisteNombreAsync(string nombre, CancellationToken ct = default)
    {
        return await _context.Productos
            .AnyAsync(p => p.producto.ToLower() == nombre.ToLower(), ct);
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int idExcluir, CancellationToken ct = default)
    {
        return await _context.Productos
            .AnyAsync(p => p.producto.ToLower() == nombre.ToLower() && p.idpro != idExcluir, ct);
    }

    // ==========================================
    // Métodos con Includes (para cargar relaciones)
    // ==========================================

    public async Task<Producto?> ObtenerConDetallesVentaAsync(int id, CancellationToken ct = default)
    {
        return await _context.Productos
            .Include(p => p.DetalleVenta)
                .ThenInclude(dv => dv.IdventaNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.idpro == id, ct);
    }

    // ==========================================
    // Operaciones en Lote
    // ==========================================

    public async Task<List<Producto>> ObtenerPorIdsAsync(List<int> ids, CancellationToken ct = default)
    {
        return await _context.Productos
            .AsNoTracking()
            .Where(p => ids.Contains(p.idpro))
            .ToListAsync(ct);
    }

    public async Task ActualizarPreciosAsync(Dictionary<int, decimal> cambiosPrecios, CancellationToken ct = default)
    {
        var ids = cambiosPrecios.Keys.ToList();
        var productos = await _context.Productos
            .Where(p => ids.Contains(p.idpro))
            .ToListAsync(ct);

        foreach (var producto in productos)
        {
            if (cambiosPrecios.TryGetValue(producto.idpro, out var nuevoPrecio))
            {
                // Usar método de dominio si existe validación
                producto.ActualizarPrecio(nuevoPrecio);
            }
        }

        await _context.SaveChangesAsync(ct);
    }
}