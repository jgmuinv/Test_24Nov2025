using Dominio.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Data.Usuarios;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // ==========================================
    // Lista para dropdownList
    // ==========================================
    public async Task<IReadOnlyList<Usuario>> ListarNombresAsync(int? idus, string? usuario, string? nombre, CancellationToken ct = default)
    {
        var query = _context.Usuarios
            .AsNoTracking()
            .AsQueryable();
        
        // Filtros solo si viene con valor
        if (idus.HasValue)
        {
            query = query.Where(p => p.idus == idus.Value);
        }
        
        if (!string.IsNullOrEmpty(usuario))
        {
            query = query.Where(p => p.usuario == usuario);
        }
        
        if (!string.IsNullOrEmpty(nombre))
        {
            query = query.Where(p => p.nombre == nombre);
        }
        
        return await query
            .OrderBy(p => p.idus)
            .ToListAsync(ct);
    }
    
}