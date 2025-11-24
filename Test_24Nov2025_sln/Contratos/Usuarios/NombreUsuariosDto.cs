using System.ComponentModel.DataAnnotations;

namespace Contratos.Usuarios;

public record NombreUsuariosDto
{
    public int IdUs { get; init; }
    public string Nombre { get; init; } = string.Empty;
}