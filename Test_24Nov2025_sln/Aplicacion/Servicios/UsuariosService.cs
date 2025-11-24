using Aplicacion.Interfaces;
using Contratos.General;
using Contratos.Productos;
using Contratos.Usuarios;
using Dominio.Common;
using Dominio.Usuarios;

namespace Aplicacion.Servicios;

public class UsuariosService : IUsuariosService
{
    private readonly IUsuarioRepository _repo;

    public UsuariosService(IUsuarioRepository repo)
    {
        _repo = repo;
    }
    
    // ==========================================
    // Lista para dropDownList
    // ==========================================
    public async Task<ResultadoDto<IReadOnlyList<NombreUsuariosDto?>>> ListarNombresAsync(int? idus, string? usuario, string? nombre, CancellationToken ct = default)
    {
        try
        {
            // Obtener todos los productos del repositorio
            var entidad = await _repo.ListarNombresAsync(idus, usuario, nombre, ct);

            // Mapear a DTOs
            var dto = entidad
                .Select(MapearANombresDto)
                .ToList();

            return ResultadoDto<IReadOnlyList<NombreUsuariosDto?>>.Success(dto.AsReadOnly());
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
    // Métodos Auxiliares
    // ==========================================

    /// <summary>
    /// Mapea una entidad a Dto
    /// </summary>
    public static NombreUsuariosDto MapearANombresDto(Usuario entidad)
    {
        return new NombreUsuariosDto
        {
            IdUs = entidad.idus,
            Nombre = entidad.nombre,
        };
    }
}